using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Scraper;
using SmartRoutes.SortaScraper.Parsers;
using SmartRoutes.SortaScraper.Support;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Model.Odjfs.ChildCares;
using Ninject.Modules;
using SmartRoutes.Graph.Node;
using System.IO;
using Ionic.Zip;
using SmartRoutes.SortaScraper.Scrapers;
using SmartRoutes.Heap;
using SmartRoutes.Database.Contexts;

namespace SmartRoutes.Graph
{
    class Program
    {
        private static Byte[] zipFileBytes = File.ReadAllBytes("C:\\DatabaseBackups\\sorta_subset_1_16.zip");

        static void Main(string[] args)
        {
            try
            {
                IKernel kernel = new StandardKernel(new GraphModule());

                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof(IEntityCollectionParser), typeof(IEntityCollectionScraper))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                //var loader = kernel.Get<DatabaseLoader>();
                //loader.loadDatabaseFromFile(zipFileBytes).Wait();

                Console.WriteLine("Creating Graph.");
                DateTime tic = DateTime.Now;

                var graph = kernel.Get<IGraph>();

                DateTime toc = DateTime.Now;

                Console.WriteLine("Graph created in {0} milliseconds.", (toc - tic).TotalMilliseconds);

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
