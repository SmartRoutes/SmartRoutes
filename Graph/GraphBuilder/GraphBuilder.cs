using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Odjfs.ChildCares;
using SortaScraper.Support;
using Model.Sorta;
using Model.Odjfs;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using NLog;
using Graph.Node;

namespace Graph
{
    public class GraphBuilder : IGraphBuilder
    {
        private readonly IMetroNode _metroNodeMaker;
        private Logger Logger = LogManager.GetCurrentClassLogger();
        private static double MaxFeetFromChildCareToBuStop = 5000; // just want to see connections get made for now
        private static double WalkingFeetPerSecond = 1.5;
        private Dictionary<int, List<int>> StopToNearest; // from StopID to list of StopID's of nearest Stops
        private Dictionary<int, List<IMetroNode>> StopToNodes; // from StopID to set of Nodes with given StopID
        Dictionary<int, List<int>> ChildCareToStops; // from ChildCare ID to closest metro stops

        public GraphBuilder(IMetroNode metroNodeMaker) 
        {
            try
            {
                _metroNodeMaker = metroNodeMaker;
                Logger.Trace("GraphBuilder object created.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during GraphBuilder object creation.", e);
                throw e;
            }
        }

        public INode[] BuildGraph(EntityCollection Collection, IEnumerable<ChildCare> ChildCares)
        {
            try
            {
                Logger.Trace("Creating new graph.");

                var MetroNodes = CreateMetroNodes(Collection);
                ConnectTrips(MetroNodes);
                //ConnectTransfers(MetroNodes);
                ConnectTransfers(MetroNodes, Collection);
                var GraphNodes = InsertChildCareNodes(Collection, ChildCares, MetroNodes);
                Logger.Trace("Graph created successfully.");
                return GraphNodes;
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during creation of graph.", e);
                Console.WriteLine(e);
                throw e;
            }
        }

        private IMetroNode[] CreateMetroNodes(EntityCollection Collection)
        {
            Logger.Trace("Creating Metro Nodes.");

            try
            {
                var MetroNodes = (from stopTime in Collection.StopTimes
                                  select _metroNodeMaker.CreateNode(stopTime))
                                 .ToArray();

                Logger.Trace("Metro Nodes created successfully.");
                return MetroNodes;
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during creation of Metro nodes.", e);
                throw e;
            }
        }

        private void ConnectTrips(IMetroNode[] MetroNodes)
        {
            Logger.Trace("Connecting Metro Trips.");
            try
            {
                Array.Sort(MetroNodes, new Comparers.ComparerForTripSorting());

                for (int i = 1; i < MetroNodes.Count(); i++)
                {
                    IMetroNode node = MetroNodes[i];
                    IMetroNode previousNode = MetroNodes[i-1];

                    if (node.TripID == previousNode.TripID)
                    {
                        node.DownwindNeighbors.Add(previousNode);
                        previousNode.UpwindNeighbors.Add(node);
                    }
                }
                Logger.Trace("Metro Trips connected successfully.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during connection of Metro Trips.", e);
                throw e;
            }
        }

        private void ConnectTransfers(IMetroNode[] MetroNodes, EntityCollection collection)
        {
            Logger.Trace("Connecting Metro Transfers.");

            try
            {
                // this sorting assures Nodes which have same stopID are sorted by ascending Time
                Array.Sort(MetroNodes, new Comparers.ComparerForTransferSorting());

                StopToNearest = new Dictionary<int, List<int>>();
                StopToNodes = new Dictionary<int, List<IMetroNode>>();

                var enumerator1 = collection.Stops.GetEnumerator();

                while (enumerator1.MoveNext())
                {
                    var Stop1 = enumerator1.Current;

                    // associate Id's of closest stops with this stop
                    StopToNearest.Add(Stop1.Id, Stop1.ChildStops.Select(s => s.Id).ToList());

                    // associate MetroNodes which contain this stop with this stop
                    var Stop1NodeList = new List<IMetroNode>();

                    for (int i = 0; i < MetroNodes.Count(); i++)
                    {
                        if (MetroNodes[i].StopID == Stop1.Id)
                        {
                            Stop1NodeList.Add(MetroNodes[i]);
                        }
                    }

                    StopToNodes.Add(Stop1.Id, Stop1NodeList);
                }

                // loop through MetroNodes and connect transfers
                foreach (var node1 in MetroNodes)
                {
                    // obtain stopID's of nodes in transfer distance
                    List<int> NearestIDs = null;
                    if (!StopToNearest.TryGetValue(node1.StopID, out NearestIDs))
                    {
                        continue;
                    }

                    foreach (var ID in NearestIDs)
                    {
                        List<IMetroNode> Nodes = null;
                        if (!StopToNodes.TryGetValue(ID, out Nodes))
                        {
                            continue;
                        }

                        if (Nodes.Count == 0) continue; // not sure if this is possible

                        // calculate walking time, will be same for all nodes in list, so use first
                        double WalkingTime = node1.GetL1DistanceInFeet(Nodes.First()) / WalkingFeetPerSecond;
                        DateTime MinTime = node1.Time + new TimeSpan(0, 0, (int)Math.Ceiling(WalkingTime));

                        // thanks to sorting, these nodes are iterated in ascending time
                        foreach (var node2 in Nodes)
                        {
                            if (node2.Time > MinTime)
                            {
                                node1.UpwindNeighbors.Add(node2);
                                node2.DownwindNeighbors.Add(node1);
                                break;
                            }
                        }
                    }
                }

                Logger.Trace("Metro Transfers connected successfully.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception encountered during connection of Metro Transfers.", e);
                throw e;
            }
        }

        private INode[] InsertChildCareNodes(EntityCollection Collection, IEnumerable<ChildCare> ChildCares, INode[] GraphNodes)
        {
            if (Collection == null || ChildCares == null || GraphNodes == null)
            {
                throw new ArgumentNullException();
            }

            var GraphNodeList = GraphNodes.ToList();

            // associate ChildCares with StopID's of closest stops
            ChildCareToStops = new Dictionary<int, List<int>>();

            var ChildCareEnumerator = ChildCares.GetEnumerator();

            while (ChildCareEnumerator.MoveNext())
            {
                var childcare = ChildCareEnumerator.Current;

                // make sure lat/long are not null
                if (childcare.Latitude == null || childcare.Longitude == null)
                {
                    continue;
                }

                var nearestStops = new List<int>();

                var StopEnumerator = Collection.Stops.GetEnumerator();

                while (StopEnumerator.MoveNext())
                {
                    var stop = StopEnumerator.Current;

                    double WalkingTime = childcare.GetL1DistanceInFeet(stop);

                    if (WalkingTime < MaxFeetFromChildCareToBuStop)
                    {
                        nearestStops.Add(stop.Id);
                    }
                }

                ChildCareToStops.Add(childcare.Id, nearestStops);
            }

            // now for each ChildCare we insert many nodes into the graph
            ChildCareEnumerator.Reset();

            while (ChildCareEnumerator.MoveNext())
            {
                var childcare = ChildCareEnumerator.Current;
                List<int> nearestStops = null;

                if (!ChildCareToStops.TryGetValue(childcare.Id, out nearestStops))
                {
                    continue;
                }

                foreach (var stop in nearestStops)
                {
                    // retrieve nodes that have to be connected to
                    List<IMetroNode> nodes = null;
                    if (!StopToNodes.TryGetValue(stop, out nodes))
                    {
                        continue;
                    }

                    // maintain list of childcare nodes added, these nodes will be
                    // connected to each other, and this connection will correspond to waiting
                    // at that childcare between two specific times
                    var ChildCareNodesAdded = new List<IChildcareNode>();

                    // make sure we have some nodes to work with
                    if (nodes.Count == 0) continue;

                    var distance = childcare.GetL1DistanceInFeet(nodes.First());
                    var walkingTime = TimeSpan.FromSeconds(distance / WalkingFeetPerSecond);

                    // for each metro node, create upwind / downwind ChildCare nodes and connect them
                    foreach (var node in nodes)
                    {
                        var downWindTime = node.Time - walkingTime;
                        var upWindTime = node.Time + walkingTime;

                        var DownWindChildCareNode = new ChildCareNode(childcare, downWindTime);
                        var UpWindChildCareNode = new ChildCareNode(childcare, upWindTime);

                        node.UpwindNeighbors.Add(UpWindChildCareNode);
                        node.DownwindNeighbors.Add(DownWindChildCareNode);
                        DownWindChildCareNode.UpwindNeighbors.Add(node);
                        UpWindChildCareNode.DownwindNeighbors.Add(node);

                        ChildCareNodesAdded.Add(DownWindChildCareNode);
                        ChildCareNodesAdded.Add(UpWindChildCareNode);
                    }

                    // add new ChildCare nodes to list of Graph nodes
                    GraphNodeList.AddRange(ChildCareNodesAdded);
                    
                    // now connect child care nodes together. Fist sort by time
                    var ChildCareNodesArray = ChildCareNodesAdded.ToArray();
                    Array.Sort(ChildCareNodesArray, new Comparers.ComparerForChildCares());

                    for (int i = 1; i < ChildCareNodesArray.Count(); i++)
                    {
                        var DownWindChildCareNode = ChildCareNodesArray[i - 1];
                        var UpWindChildCareNode = ChildCareNodesArray[i];

                        DownWindChildCareNode.UpwindNeighbors.Add(UpWindChildCareNode);
                        UpWindChildCareNode.DownwindNeighbors.Add(DownWindChildCareNode);
                    }
                }
            }

            return GraphNodeList.ToArray();
        }
    }
}
