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

        private static void Main(string[] args)
        {
            try
            {
                Logger.Trace("SortaDataChecker is now starting.");

                IKernel kernel = new StandardKernel();
                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof (IEntityCollectionScraper))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                var dataChecker = new DataChecker(kernel.Get<IEntityCollectionScraper>());
                dataChecker.UpdateDatabase().Wait();

                Logger.Trace("SortaDataChecker has completed.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("An exception has forced the SortaDataChecker to terminate.", e);
            }
        }
    }
}