using System;
using Ninject;
using Ninject.Extensions.Conventions;
using NLog;
using SortaScraper.Scrapers;

namespace SortaDataChecker
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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