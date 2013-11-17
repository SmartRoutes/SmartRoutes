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
        private static Byte[] zipFileBytes = File.ReadAllBytes("C:\\Users\\alcaz0r\\Documents\\School\\CS Senior Design\\streetsmartz\\Sandbox\\sorta\\google_transit_info.zip");

        static void Main(string[] args)
        {
            try
            {
                IKernel kernel = new StandardKernel(new GraphModule());

                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof(IEntityCollectionParser), typeof(IEntityCollectionScraper))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                using (var ctx = new OdjfsEntities())
                {
                    var childcares = (from c in ctx.ChildCares select c).ToList();

                    foreach (var c in childcares)
                    {
                        Console.WriteLine(c.Address);
                    }
                }

                // initialize database to small zip file
                //DatabaseLoader dbloader = new DatabaseLoader(kernel.Get<IEntityCollectionParser>());
                //dbloader.loadDatabaseFromFile(zipFileBytes).Wait();

                //Console.WriteLine("Creating Graph.");
                //DateTime tic = DateTime.Now;

                //var graph = kernel.Get<IGraph>();

                //DateTime toc = DateTime.Now;

                //Console.WriteLine("Graph created in {0} milliseconds.", (toc - tic).TotalMilliseconds);

                //var heap = new FibonacciHeap<double, double>();

                //int count = 100000;
                //var rand = new Random();
                //var handles = new FibHeapHandle<double, double>[count];

                //for (int i = 0; i < count; i++)
                //{
                //    var num1 = rand.NextDouble();
                //    var num2 = rand.NextDouble();
                //    handles[i] = heap.Insert(num1, num1);
                //}

                //while (!heap.Empty())
                //{
                //    //heap.DeleteMin();
                //    Console.WriteLine(heap.DeleteMin());
                //}

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
