﻿using System;
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
using Ninject.Modules;
using Graph.Node;
using System.IO;
using Ionic.Zip;
using SortaScraper.Scrapers;
using Heap;

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

                // initialize database to small zip file
                //DatabaseLoader dbloader = new DatabaseLoader(kernel.Get<IEntityCollectionParser>());
                //dbloader.loadDatabaseFromFile(zipFileBytes).Wait();

                Console.WriteLine("Creating Graph.");
                DateTime tic = DateTime.Now;

                var graph = kernel.Get<IGraph>();

                DateTime toc = DateTime.Now;

                Console.WriteLine("Nodes created in {0} milliseconds.", (toc - tic).TotalMilliseconds);
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