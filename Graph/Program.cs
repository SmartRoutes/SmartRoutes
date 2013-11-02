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
using Ninject.Modules;
using Graph.Node;
using System.IO;
using Ionic.Zip;

namespace Graph
{
    class Program
    {
        private static Byte[] zipFileBytes = File.ReadAllBytes("C:\\Users\\alcaz0r\\Documents\\School\\CS Senior Design\\streetsmartz\\Sandbox\\sorta\\google_transit_info.zip");

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("GraphBuildingTester is now starting.");

                Console.WriteLine("Attempting to parse SORTA zip file.");

                IKernel kernel = new StandardKernel(new GraphModule());

                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof(IEntityCollectionParser))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                var collection = kernel.Get<IEntityCollectionParser>().Parse(zipFileBytes);

                Console.WriteLine("SORTA zip file successfully parsed.");

                Console.WriteLine("Stitching collection entities together.");

                foreach (StopTime entry in collection.StopTimes)
                {
                    entry.Stop = collection.Stops.
                        Single<Stop>(s => s.Id == entry.StopId);
                }

                foreach (StopTime entry in collection.StopTimes)
                {
                    entry.Trip = collection.Trips.
                        Single<Trip>(s => s.Id == entry.TripId);
                }

                foreach (Trip entry in collection.Trips)
                {
                    entry.Shape = collection.Shapes.
                        Single<Shape>(s => s.Id == entry.ShapeId);
                }

                Console.WriteLine("Creating Nodes.");
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
