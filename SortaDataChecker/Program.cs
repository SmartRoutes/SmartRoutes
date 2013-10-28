using System;
using Ninject;
using Ninject.Extensions.Conventions;
using NLog;
using SortaScraper.Scrapers;
using SortaScraper.Parsers;
using Scraper;
using Database;
using Database.Contexts;
using System.Threading.Tasks;
using Model.Sorta;
using System.Data.Entity;
using System.Linq;

namespace SortaDataChecker
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static SortaScraper.Support.EntityCollection getCollection(IEntityCollectionParser parser)
        {
            return parser.Parse(new byte[(long) Math.Pow(2,13)]);
        }

/*        private static async Task insertCollection(SortaScraper.Support.EntityCollection collection)
        {
            using (var ctx = new SortaEntities())
            {
                Archive currentArchive = await ctx.Archives
                    .OrderByDescending(a => a.DownloadedOn)
                    .FirstOrDefaultAsync();

                using (var inserter = new FastInserter<SortaEntities>(ctx, 1000))
                {
                    try
                    {
                        Console.WriteLine("Begin Insertion.");
                        await inserter.AddRangeAsync(collection.Services);
                        Console.WriteLine("Services Inserted.");
                        await inserter.AddRangeAsync(collection.ServiceException);
                        Console.WriteLine("ServiceException Inserted.");
                        await inserter.AddRangeAsync(collection.Routes);
                        Console.WriteLine("Routes Inserted.");
                        await inserter.AddRangeAsync(collection.Agencies);
                        Console.WriteLine("Agencies Inserted.");
                        await inserter.AddRangeAsync(collection.Shapes);
                        Console.WriteLine("Shapes Inserted.");
//                        await inserter.AddRangeAsync(collection.ShapePoints);
//                        Console.WriteLine("ShapePoints Inserted.");
 //                       await inserter.AddRangeAsync(collection.Stops);
 //                       Console.WriteLine("Stops Inserted.");
//                        await inserter.AddRangeAsync(collection.StopTimes);
//                        Console.WriteLine("StopTimes Inserted.");
//                        await inserter.AddRangeAsync(collection.Trips);
//                        Console.WriteLine("Trips Inserted.");
                    }
                    catch
                    {
                        Console.WriteLine("Couldn't insert stuff.");
                    }
                }
            }
        } */

        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Doing something.");
                Logger.Trace("SortaDataChecker is now starting.");

                IKernel kernel = new StandardKernel();
                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof (IEntityCollectionScraper))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                IKernel parserKernel = new StandardKernel();
                parserKernel.Bind(c => c
                    .FromAssemblyContaining(typeof (IEntityCollectionParser))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                Console.WriteLine("Parsing.");
                var collection = getCollection(parserKernel.Get<IEntityCollectionParser>());

//                Console.WriteLine("Inserting collection into database.");
//               insertCollection(collection).Wait();

                Console.WriteLine("Starting DataChecker.");
                var dataChecker = new DataChecker(kernel.Get<IEntityCollectionScraper>());
                dataChecker.UpdateDatabase().Wait();

                Logger.Trace("SortaDataChecker has completed.");
                Console.WriteLine("Did something.");
                var nubdrip = Console.ReadLine();
            }
            catch (Exception e)
            {
                Logger.ErrorException("An exception has forced the SortaDataChecker to terminate.", e);
            }
        }
    }
}