using System;
using System.Linq;
using Database;
using Database.Contexts;
using Model.Sorta;
using Ninject;
using Ninject.Extensions.Conventions;
using SortaScraper.Scrapers;
using SortaScraper.Support;

namespace SortaDataChecker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind(c => c
                .FromAssemblyContaining(typeof (IEntityCollectionScraper))
                .SelectAllClasses()
                .BindAllInterfaces());

            using (var ctx = new SortaContext())
            {
                // improve performance on adding
                ctx.Configuration.AutoDetectChangesEnabled = false;
                ctx.Configuration.ValidateOnSaveEnabled = false;

                Console.Write("Getting the currently loaded archive...");
                Archive currentArchive = ctx.Archives
                    .OrderByDescending(a => a.DownloadedOn)
                    .FirstOrDefault();
                Console.WriteLine(" done.");

                var scraper = kernel.Get<IEntityCollectionScraper>();
                Console.Write("Checking the newest archive...");
                EntityCollection entityCollection = scraper.Scrape(currentArchive).Result;
                Console.WriteLine(" done.");

                // handled matching ETag and LastModified headers
                if (entityCollection == null)
                {
                    Console.WriteLine("The currently loaded archive is up to date.");

                    return;
                }

                // handle hash match
                if (!entityCollection.ContainsEntities)
                {
                    Console.WriteLine("The ETag or LastModified headers did not match, but the archive itself has not changed.");

                    Console.Write("Adding the new archive record...");
                    ctx.Archives.Add(entityCollection.Archive);
                    ctx.SaveChanges();
                    Console.WriteLine(" done.");

                    return;
                }

                // handle new data
                Console.WriteLine("A new archive has been released.");

                Console.Write("Truncating the existing data...");
                ctx.Truncate();
                Console.WriteLine(" done.");

                using (var inserter = new FastInserter<SortaContext>(ctx, 1000))
                {
                    Console.Write("Adding the agencies...");
                    inserter.AddRange(entityCollection.Agencies);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the services...");
                    inserter.AddRange(entityCollection.Services);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the service exceptions...");
                    inserter.AddRange(entityCollection.ServiceException);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the routes...");
                    inserter.AddRange(entityCollection.Routes);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the shapes...");
                    inserter.AddRange(entityCollection.Shapes);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the shape points...");
                    inserter.AddRange(entityCollection.ShapePoints);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the blocks...");
                    inserter.AddRange(entityCollection.Blocks);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the trips...");
                    inserter.AddRange(entityCollection.Trips);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the stops...");
                    inserter.AddRange(entityCollection.Stops);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the stop times...");
                    inserter.AddRange(entityCollection.StopTimes);
                    Console.WriteLine(" done.");

                    Console.Write("Adding the archive record...");
                    inserter.Add(entityCollection.Archive);
                    Console.WriteLine(" done.");
                }
            }
        }
    }
}