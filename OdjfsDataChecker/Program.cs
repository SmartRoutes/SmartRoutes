using System;
using Database.Contexts;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Parameters;
using NLog;
using OdjfsScraper.Scrapers;
using OdjfsScraper.Support;

namespace OdjfsDataChecker
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            try
            {
                Logger.Trace("OdjfsDataChecker is now starting.");

                IKernel kernel = new StandardKernel();
                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof (IChildCareStubListScraper))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                var parameter = new ConstructorArgument("odjfsClient", new DownloadingOdjfsClient("HTML"));
                var dataChecker = new Odjfs(kernel.Get<IChildCareStubListScraper>(parameter), kernel.Get<IChildCareScraper>(parameter));

                using (var ctx = new OdjfsEntities())
                {
                    // dataChecker.UpdateNextCounty(ctx).Wait();
                    dataChecker.UpdateNextChildCare(ctx).Wait();
                    ctx.SaveChanges();
                }
                
                Logger.Trace("SortaDataChecker has completed.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("An exception has forced the SortaDataChecker to terminate.", e);
            }
        }
    }
}