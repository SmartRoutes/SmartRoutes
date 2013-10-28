using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Contexts;
using Model.Sorta;
using NLog;
using SortaScraper.Scrapers;
using SortaScraper.Support;

namespace SortaDataChecker
{
    public class DataChecker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEntityCollectionScraper _scraper;

        public DataChecker(IEntityCollectionScraper scraper)
        {
            _scraper = scraper;
        }

        public async Task UpdateDatabase()
        {
            Logger.Trace("Initializing SortaEntities.");
            using (var ctx = new SortaEntities())
            {
                // get the current archive from the database (if any)
                Logger.Trace("Getting the current Archive instance (if any).");
                Archive currentArchive = await ctx.Archives
                    .OrderByDescending(a => a.DownloadedOn)
                    .FirstOrDefaultAsync();
                if (currentArchive == null)
                {
                    Logger.Trace("No current archive is recorded.");
                }
                else
                {
                    Logger.Trace("The current archive was downloaded on {0}.", currentArchive.DownloadedOn);
                }

                Logger.Trace("Fetching the entity collection.");
                EntityCollection entityCollection = await _scraper.Scrape(currentArchive);

                // handled matching ETag and LastModified headers
                if (entityCollection == null)
                {
                    Logger.Trace("No entity collection was returned.");
                    return;
                }

                // handle hash match
                if (!entityCollection.ContainsEntities)
                {
                    Logger.Trace("No entities were returned except for a new Archive instance.");
                    ctx.Archives.Add(entityCollection.Archive);
                    Logger.Trace("Saving the new Archive record.");
                    await ctx.SaveChangesAsync();
                    return;
                }

                // truncate the old data
                Logger.Trace("Truncating all of the old entities.");
                ctx.Truncate();

                using (var inserter = new FastInserter<SortaEntities>(ctx, 1000))
                {
                    //  persist all of the new entities
                    Logger.Trace("Adding {0} new Agency records.", entityCollection.Agencies.Count());
                    await inserter.AddRangeAsync(entityCollection.Agencies);

                    Logger.Trace("Adding {0} new Service records.", entityCollection.Services.Count());
                    await inserter.AddRangeAsync(entityCollection.Services);

                    Logger.Trace("Adding {0} new ServiceException records.", entityCollection.ServiceExceptions.Count());
                    await inserter.AddRangeAsync(entityCollection.ServiceExceptions);

                    Logger.Trace("Adding {0} new Route records.", entityCollection.Routes.Count());
                    await inserter.AddRangeAsync(entityCollection.Routes);

                    Logger.Trace("Adding {0} new Shape records.", entityCollection.Shapes.Count());
                    await inserter.AddRangeAsync(entityCollection.Shapes);

                    Logger.Trace("Adding {0} new ShapePoint records.", entityCollection.ShapePoints.Count());
                    await inserter.AddRangeAsync(entityCollection.ShapePoints);

                    Logger.Trace("Adding {0} new Block records.", entityCollection.Blocks.Count());
                    await inserter.AddRangeAsync(entityCollection.Blocks);

                    Logger.Trace("Adding {0} new Trip records.", entityCollection.Trips.Count());
                    await inserter.AddRangeAsync(entityCollection.Trips);

                    Logger.Trace("Adding {0} new Stop records.", entityCollection.Stops.Count());
                    await inserter.AddRangeAsync(entityCollection.Stops);

                    Logger.Trace("Adding {0} new StopTime records.", entityCollection.StopTimes.Count());
                    await inserter.AddRangeAsync(entityCollection.StopTimes);

                    Logger.Trace("Adding the new Archive record.");
                    await inserter.AddAsync(entityCollection.Archive);

                    await inserter.SaveChangesAsync();
                }
            }
        }
    }
}