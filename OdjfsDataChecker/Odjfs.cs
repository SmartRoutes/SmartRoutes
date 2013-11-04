using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Database.Contexts;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;
using NLog;
using OdjfsScraper.Scrapers;
using Scraper;

namespace OdjfsDataChecker
{
    public class Odjfs
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IChildCareScraper _childCareScraper;
        private readonly IChildCareStubListScraper _listScraper;

        public Odjfs(IChildCareStubListScraper listScraper, IChildCareScraper childCareScraper)
        {
            _listScraper = listScraper;
            _childCareScraper = childCareScraper;
        }

        public async Task UpdateChildCareStub(OdjfsEntities ctx, ChildCareStub stub)
        {
            // record this scrape
            stub.LastScrapedOn = DateTime.Now;
            await ctx.SaveChangesAsync();

            Logger.Trace("Stub with ID '{0}' will be scraped.", stub.ExternalUrlId);
            ChildCare newChildCare = await _childCareScraper.Scrape(stub);
            ctx.ChildCareStubs.Remove(stub);
            if (newChildCare != null)
            {
                SetAttachedCounty(ctx, newChildCare);
                ctx.ChildCares.AddOrUpdate(newChildCare);
            }
            else
            {
                ChildCare existingChildCare = await ctx
                    .ChildCares
                    .Where(c => c.ExternalUrlId == stub.ExternalUrlId)
                    .FirstOrDefaultAsync();
                if (existingChildCare != null)
                {
                    ctx.ChildCares.Remove(existingChildCare);
                }
                Logger.Trace("The full detail page for the stub could not be found so the child care was deleted.");
            }

            Logger.Trace("Saving changes.");
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateChildCare(OdjfsEntities ctx, ChildCare oldChildCare)
        {
            // record this scrape
            oldChildCare.LastScrapedOn = DateTime.Now;
            await ctx.SaveChangesAsync();

            Logger.Trace("Child care with ID '{0}' will be scraped.", oldChildCare.ExternalUrlId);
            ChildCare newChildCare = await _childCareScraper.Scrape(oldChildCare);
            if (newChildCare != null)
            {
                SetAttachedCounty(ctx, newChildCare);
                ctx.ChildCares.AddOrUpdate(newChildCare);
            }
            else
            {
                ctx.ChildCares.Remove(oldChildCare);
                ChildCareStub stub = await ctx
                    .ChildCareStubs
                    .Where(c => c.ExternalUrlId == oldChildCare.ExternalUrlId)
                    .FirstOrDefaultAsync();
                if (stub != null)
                {
                    ctx.ChildCareStubs.Remove(stub);
                }
                Logger.Trace("The full detail page for the child care could not be found so the child care was deleted.");
            }

            Logger.Trace("Saving changes.");
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateNextChildCare(OdjfsEntities ctx)
        {
            Logger.Trace("Getting the next child care to scrape.");
            Logger.Trace("Checking for a stub that need to be scraped.");
            ChildCareStub stub = await ctx
                .ChildCareStubs
                .OrderBy(c => c.LastScrapedOn.HasValue)
                .ThenByDescending(c => c.LastScrapedOn)
                .FirstOrDefaultAsync();
            if (stub != null)
            {
                await UpdateChildCareStub(ctx, stub);
            }
            else
            {
                Logger.Trace("No stub was found, so a child care will be checked for updates.");
                ChildCare childCare = await ctx
                    .ChildCares
                    .OrderBy(c => c.LastScrapedOn)
                    .FirstOrDefaultAsync();

                if (childCare == null)
                {
                    Logger.Trace("There are no child care or child care stub records to scrape.");
                    return;
                }

                await UpdateChildCare(ctx, childCare);
            }
        }

        public async Task UpdateNextCounty(OdjfsEntities ctx)
        {
            Logger.Trace("Getting the next County to scrape.");
            County county = ctx
                .Counties
                .OrderBy(c => c.LastScrapedOn.HasValue)
                .ThenByDescending(c => c.LastScrapedOn)
                .First();
            Logger.Trace("The next county to scrape is '{0}'.", county.Name);

            await UpdateCounty(ctx, county);
        }

        public async Task UpdateCounty(OdjfsEntities ctx, County county)
        {
            // record this scrape
            county.LastScrapedOn = DateTime.Now;
            await ctx.SaveChangesAsync();

            // get the stubs from the web
            Logger.Trace("Scraping stubs for county '{0}'.", county.Name);
            ChildCareStub[] webStubs = (await _listScraper.Scrape(county)).ToArray();
            Logger.Trace("{0} stubs were scraped.", webStubs.Length);

            // get the IDs
            ISet<string> webIds = new HashSet<string>(webStubs.Select(c => c.ExternalUrlId));

            if (webStubs.Length != webIds.Count)
            {
                IEnumerable<string> duplicateIds = webStubs
                    .Select(c => c.ExternalUrlId)
                    .GroupBy(i => i)
                    .Select(g => g.ToArray())
                    .Where(g => g.Length > 0)
                    .Select(g => g[0]);

                var exception = new ScraperException("One more more duplicate child cares were found in the list.");
                Logger.ErrorException(string.Format(
                    "County: '{0}', HasDuplicates: '{1}', TotalCount: {2}, UniqueCount: {3}",
                    county.Name,
                    string.Join(", ", duplicateIds),
                    webStubs.Length,
                    webIds.Count), exception);
                throw exception;
            }

            // get all of the external IDs that could belong to this county
            ISet<string> dbStubIds = new HashSet<string>(await ctx
                .ChildCareStubs
                .Where(c => c.CountyId == null || c.CountyId == county.Id)
                .Select(c => c.ExternalUrlId)
                .ToArrayAsync());
            Logger.Trace("{0} stubs were found in the database.", dbStubIds.Count);

            ISet<string> dbIds = new HashSet<string>(await ctx
                .ChildCares
                .Where(c => c.CountyId == county.Id)
                .Select(c => c.ExternalUrlId)
                .ToArrayAsync());
            Logger.Trace("{0} child cares were found in the database.", dbIds.Count);

            if (dbStubIds.Overlaps(dbIds))
            {
                dbStubIds.IntersectWith(dbIds);

                var exception = new ScraperException("There are child cares that exist in both the ChildCare and ChildCareStub tables.");
                Logger.ErrorException(string.Format("County: '{0}', Overlapping: '{1}'", county.Name, string.Join(", ", dbStubIds)), exception);
                throw exception;
            }

            dbIds.UnionWith(dbStubIds);

            // find the newly deleted child cares
            ISet<string> deleted = new HashSet<string>(dbIds);
            deleted.ExceptWith(webIds);
            Logger.Trace("{0} child cares or stubs will be deleted.", deleted.Count);

            // delete
            if (deleted.Count > 0)
            {
                // TODO: keep an eye on https://github.com/loresoft/EntityFramework.Extended/issues/62#issuecomment-25361505
                IEnumerable<SqlParameter> parameters = deleted.Select((id, i) => new SqlParameter("@p" + i, id));
                string query = "DELETE FROM {0} WHERE ExternalUrlId IN (" + string.Join(", ", parameters.Select(p => p.ParameterName)) + ")";

                // intentionally enumerate the parameters twice, to get new SqlParameter instances per query
                await ctx.Database.ExecuteSqlCommandAsync(string.Format(query, "odjfs.ChildCareStub"), parameters.ToArray());
                await ctx.Database.ExecuteSqlCommandAsync(string.Format(query, "odjfs.ChildCare"), parameters.ToArray());
            }

            // find the newly added child cares
            ISet<string> added = new HashSet<string>(webIds);
            added.ExceptWith(dbIds);
            Logger.Trace("{0} stubs will be added.", added.Count);

            // add
            foreach (ChildCareStub stub in webStubs.Where(c => added.Contains(c.ExternalUrlId)))
            {
                ctx.ChildCareStubs.Add(stub);
            }

            Logger.Trace("Saving changes.");
            await ctx.SaveChangesAsync();
        }

        private void SetAttachedCounty(OdjfsEntities ctx, ChildCare childCare)
        {
            if (childCare.County != null && childCare.County.Id == 0)
            {
                childCare.County = ctx.GetAttachedCounty(childCare.County.Name);
                childCare.CountyId = childCare.County.Id;
            }
        }
    }
}