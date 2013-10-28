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

namespace GraphBuildingTester
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private class NodeModule : NinjectModule
        {
            public override void Load()
            {
                Bind<INode>().To<Node>();
            }
        }

        private static EntityCollection parseCollection(IEntityCollectionParser parser)
        {
            return parser.Parse(new byte[8192]);
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("GraphBuildingTester is now starting.");

                Console.WriteLine("Attempting to parse SORTA zip file.");

                IKernel kernel = new StandardKernel(new NodeModule());

                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof(IEntityCollectionParser))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                var collection = parseCollection(kernel.Get<IEntityCollectionParser>());

                Console.WriteLine("SORTA zip file successfully parsed.");

                Console.WriteLine("Assigning Stops to StopTime entities.");

                foreach (StopTime entry in collection.StopTimes)
                {
                    entry.Stop = collection.Stops.
                        Single<Stop>(s => s.Id == entry.StopId);
                }

                Console.WriteLine("Creating Nodes.");

                var MetroNodes = collection.Trips.
                    SelectMany<Trip, INode>(trip => (
                        from stopTime in collection.StopTimes
                        where stopTime.TripId == trip.Id
                        orderby stopTime.Sequence ascending
                        select kernel.Get<INode>().CreateNode(stopTime)
                        ));

                Console.WriteLine("Connecting Trips.");

                // connect MetroNodes on same route
                INode previousNode = null;
                int count = 0;
                foreach (INode node in MetroNodes)
                {
                    if (count == 0)
                    {
                        count++;
                    }
                    else if (node.TripID == previousNode.TripID)
                    {
                        node.addDownwindNeighbor(previousNode);
                        previousNode.addUpwindNeighbor(node);
                    }
                    previousNode = node;
                }

                Console.WriteLine("Trips connected.");
//                Console.WriteLine(String.Format("{0}", MetroNodes.Count()));
//                Console.WriteLine(String.Format("{0}", collection.StopTimes.Count()));
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                Logger.ErrorException("An exception has forced GraphBuildingTester to ternminate.", e);
            }
        }
    }
}
