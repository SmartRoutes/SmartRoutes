using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Database;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;

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
                    .FromAssemblyContaining(typeof(GtfsCollection))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                /********************************************************************************************
                ******************* Fetch the data from the database and build the graph ********************
                ********************************************************************************************/

                Console.WriteLine("Fetching the GTFS data from the database.");
                DateTime tic = DateTime.Now;
                var gtfsCollection = new GtfsCollection();
                using (var ctx = new Entities())
                {
                    gtfsCollection.StopTimes = (from e in ctx.StopTimes select e).ToArray();
                    gtfsCollection.Stops = (from e in ctx.Stops select e).ToArray();
                    gtfsCollection.Routes = (from e in ctx.Routes select e).ToArray();
                    gtfsCollection.Shapes = (from e in ctx.Shapes select e).ToArray();
                    gtfsCollection.ShapePoints = (from e in ctx.ShapePoints select e).ToArray();
                    gtfsCollection.Blocks = (from e in ctx.Blocks select e).ToArray();
                    gtfsCollection.Agencies = (from e in ctx.Agencies select e).ToArray();
                    gtfsCollection.Archive = ctx.GtfsArchives.OrderBy(e => e.LoadedOn).FirstOrDefault();
                    gtfsCollection.Trips = (from e in ctx.Trips select e).ToArray();
                    gtfsCollection.ServiceExceptions = (from e in ctx.ServiceExceptions select e).ToArray();
                    gtfsCollection.Services = (from e in ctx.Services select e).ToArray();
                    gtfsCollection.ContainsEntities = true;
                }
                DateTime toc = DateTime.Now;
                Console.WriteLine("GTFS data fetched in {0} milliseconds.", (toc - tic).TotalMilliseconds);
                
                Console.WriteLine("Fetching the destination data from the database.");
                tic = DateTime.Now;
                var srdsCollection = new SrdsCollection();

                using (var ctx = new Entities())
                {
                    srdsCollection.AttributeValues = (from e in ctx.AttributeValues select e).ToArray();
                    srdsCollection.Destinations = (from e in ctx.Destinations select e).ToArray();
                    srdsCollection.AttributeKeys = (from e in ctx.AttributeKeys select e).ToArray();
                }
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

                // child care selected by name, since properties are not implemented yet
                const string childCareName = "ANOINTED HANDS LEARNING CENTER";

                Console.WriteLine("getting workNodes.");
                // search starts at work, going backwards
                var workNodes = graph.GetClosestGtfsNodes(workLocation, atWorkBy, TimeDirection.Backwards);

                // since we don't have properties on our location nodes yet, let's just filter by name.
                // this returns two results (apparently there are two child cares with this name)
                Func<INode, bool> goalCheck = node =>
                {
                    var nodeAsChildCare = node as DestinationNode;
                    if (nodeAsChildCare != null)
                    {
                        return nodeAsChildCare.Name == childCareName;
                    }
                    return false;
                };

                Console.WriteLine("getting workToChildCareResults.");
                var workToChildCareResults = ExtensionMethods.Dijkstras(workNodes, goalCheck, TimeDirection.Backwards);

                Console.WriteLine("getting closeToHomeNodes.");
                // First step, find which bus stop is closest to my house, and set that as destination
                var closeToHomeNodes = graph.GetClosestGtfsNodes(homeLocation, workToChildCareResults.First().node.Time, TimeDirection.Backwards);

                Func<INode, bool> goalCheck2 = node =>
                {
                    var nodeAsGtfsNode = node as IGtfsNode;
                    if (nodeAsGtfsNode != null)
                    {
                        bool match = false;
                        foreach (var n in closeToHomeNodes)
                        {
                            if (nodeAsGtfsNode.StopId == n.StopId)
                            {
                                match = true;
                                break;
                            }
                        }
                        return match;
                    }
                    return false;
                };

                // now for each child care result, we find our way home.
                var finalResults = new List<NodeInfo>();
                var result = workToChildCareResults.First();

                Console.WriteLine("getting startNodes.");
                //var startNodes = graph.GetDestinationNeighbors((IDestinationNode)result.node, TimeDirection.Backwards);
                var startNodes = graph.GetClosestGtfsNodes(result.node, result.node.Time, TimeDirection.Backwards);

                Console.WriteLine("getting resultList.");
                var resultList = ExtensionMethods.Dijkstras(
                    startNodes,
                    goalCheck2,
                    TimeDirection.Backwards);

                var result2 = resultList.First();

                Console.WriteLine("stitching results.");
                // we want to stich together the two routes to make one resulting route
                var current = result2;
                while (current.parent != null) current = current.parent;
                current.parent = result;

                finalResults.Add(result2);

                toc = DateTime.Now;
                Console.WriteLine("Route found in in {0} milliseconds.",
                    (toc - tic).TotalMilliseconds);

                Console.WriteLine("Displaying Route.");
                current = finalResults.First();
                while (current != null)
                {
                    var gtfsCurrent = current.node as IGtfsNode;
                    if (gtfsCurrent != null)
                    {
                        Console.WriteLine("{0} -- {1} <Trip {2}> <Route {3}>", current.node.Name, current.node.Time, gtfsCurrent.TripId, gtfsCurrent.RouteId);
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
