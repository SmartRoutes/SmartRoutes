using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.Graph
{
    struct Location : ILocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IKernel kernel = new StandardKernel(new GraphModule());

                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof(GtfsCollection), typeof(IEntityCollectionDownloader<,>))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                /********************************************************************************************
                ******************* Fetch the data from the database and build the graph ********************
                ********************************************************************************************/

                Console.WriteLine("Fetching the GTFS data from the web.");
                DateTime tic = DateTime.Now;
                var gtfsFetcher = kernel.Get<IEntityCollectionDownloader<GtfsArchive, GtfsCollection>>();
                var gtfsCollection = gtfsFetcher.Download(new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"), null).Result;
                DateTime toc = DateTime.Now;
                Console.WriteLine("GTFS data fetched in {0} milliseconds.", (toc - tic).TotalMilliseconds);
                
                Console.WriteLine("Fetching the destination data from the web.");
                tic = DateTime.Now;
                var srdsFetcher = kernel.Get<IEntityCollectionDownloader<SrdsArchive, SrdsCollection>>();
                var srdsCollection = srdsFetcher.Download(new Uri(SRDS_URL), null).Result;
                toc = DateTime.Now;
                Console.WriteLine("Destination data fetched in {0} milliseconds.", (toc - tic).TotalMilliseconds);

                Console.WriteLine("Creating Graph.");
                tic = DateTime.Now;
                var graphBuilder = kernel.Get<IGraphBuilder>();
                var graph = graphBuilder.BuildGraph(gtfsCollection.StopTimes, srdsCollection.Destinations, GraphBuilderSettings.Default);
                toc = DateTime.Now;
                Console.WriteLine("Graph created in {0} milliseconds.", (toc - tic).TotalMilliseconds);
                Console.WriteLine("Finding route...");
                tic = DateTime.Now;

                /********************************************************************************************
                ******************** Example usage of Dijkstras for complete route **************************
                ********************************************************************************************/
                                
                // starting at my address
                var homeLocation = new Location { Latitude = 39.122309, Longitude = -84.507639 };

                // ending at the college of engineering
                var workLocation = new Location { Latitude = 39.133292, Longitude = -84.515099 };

                // have to be at work by 10:30 am
                var atWorkBy = new DateTime(1970, 1, 1, 10, 30, 0);

                // find child cares close to home, choose names from them
                Func<IDestination, double> ordering = node => node.GetL1DistanceInFeet(homeLocation);
                var closeChildCares = srdsCollection.Destinations.OrderBy(ordering);

                // child care selected by name, since properties are not implemented yet
                const string childCareName1 = "ANOINTED HANDS LEARNING CENTER";
                const string childCareName2 = "DIVINE DAY CARE CENTER, INC. II";

                Func<IDestination, bool> Criteria1 = dest => dest.Name == childCareName1;
                Func<IDestination, bool> Criteria2 = dest => dest.Name == childCareName2;

                var Criterion = new[] { Criteria1, Criteria2 };

                var results = graph.Search(workLocation, homeLocation, atWorkBy, TimeDirection.Backwards, Criterion);

                toc = DateTime.Now;
                Console.WriteLine("Route found in in {0} milliseconds.",
                    (toc - tic).TotalMilliseconds);

                var current = results.First();
                Console.WriteLine("Displaying Route.");
                while (current != null)
                {
                    var gtfsCurrent = current.node as IGtfsNode;
                    if (gtfsCurrent != null)
                    {
                        Console.WriteLine("{0} -- {1} <Trip {2}> <Route {3}>", current.node.Name, current.node.Time, gtfsCurrent.TripId, gtfsCurrent.stopTime.Trip.Route.ShortName);
                    }
                    else
                    {
                        Console.WriteLine("{0} -- {1}", current.node.Name, current.node.Time);
                    }
                    current = current.parent;
                }

                Console.WriteLine("fin");



                //DateTime toc = DateTime.Now;

                //Console.WriteLine("Graph created in {0} milliseconds.", (toc - tic).TotalMilliseconds);
                //Console.WriteLine("Performing Dijkstras...");
                //tic = DateTime.Now;

                //int count = 0;

                //Func<INode, bool> GoalCheck = node =>
                //{
                //    var check = node as DestinationNode;
                //    return check != null;
                //};

                //var Results = new List<NodeInfo>();

                //var StartNodes = new INode[] { graph.GraphNodes[count] };
                //Results = ExtensionMethods.Dijkstras(StartNodes, GoalCheck, TimeDirection.Forwards);

                //toc = DateTime.Now;
                //Console.WriteLine("Dijkstra's completed in {0} milliseconds, {1} results found.", 
                //    (toc - tic).TotalMilliseconds, Results.Count());

                //List<NodeBase> UniqueChildCareBases = new List<NodeBase>();

                //foreach (var node in graph.GraphNodes)
                //{
                //    var cnode = node as DestinationNode;
                //    if (cnode == null) continue;
                //    if (!UniqueChildCareBases.Contains(cnode.BaseNode))
                //    {
                //        UniqueChildCareBases.Add(cnode.BaseNode);
                //    }
                //}

                //Console.WriteLine("{0} unique child cares found in graph.", UniqueChildCareBases.Count());

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
