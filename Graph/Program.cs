using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Ninject;
using Ninject.Extensions.Conventions;
using Scraper;
using SortaScraper.Parsers;
using SortaScraper.Support;
using Model.Sorta;
using Model.Odjfs.ChildCares;
using Ninject.Modules;
using Graph.Node;
using System.IO;
using Ionic.Zip;
using SortaScraper.Scrapers;
using Heap;
using Database.Contexts;

namespace Graph
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
