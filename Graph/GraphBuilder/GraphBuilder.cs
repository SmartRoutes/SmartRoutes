using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.SortaScraper.Support;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Model.Odjfs;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using NLog;

namespace SmartRoutes.Graph
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
            _metroNodeMaker = metroNodeMaker;
            Logger.Trace("GraphBuilder object created.");
        }

        public INode[] BuildGraph(IEnumerable<StopTime> StopTimes, IEnumerable<ChildCare> ChildCares)
        {
            Logger.Trace("Creating new graph.");

            // collect all the stops
            Stop[] Stops = StopTimes
                .GroupBy(s => s.StopId)
                .Select(g => g.First().Stop)
                .ToArray();

            var MetroNodes = CreateMetroNodes(StopTimes);
            ConnectTrips(MetroNodes);
            //ConnectTransfers(MetroNodes);
            ConnectTransfers(MetroNodes, Stops);
            var GraphNodes = InsertChildCareNodes(Stops, ChildCares, MetroNodes);
            Logger.Trace("Graph created successfully.");
            return GraphNodes;
        }

        private IMetroNode[] CreateMetroNodes(IEnumerable<StopTime> stopTimes)
        {
            Logger.Trace("Creating Metro Nodes.");

            var MetroNodes = (from stopTime in stopTimes
                                select _metroNodeMaker.CreateNode(stopTime))
                                .ToArray();

            Logger.Trace("Metro Nodes created successfully.");
            return MetroNodes;
        }

        private void ConnectTrips(IMetroNode[] MetroNodes)
        {
            Logger.Trace("Connecting Metro Trips.");

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

        private void ConnectTransfers(IMetroNode[] MetroNodes, Stop[] Stops)
        {
            Logger.Trace("Connecting Metro Transfers.");

            // sorts Nodes first by ascending stopID, second by ascending Time
            Array.Sort(MetroNodes, new Comparers.ComparerForTransferSorting());

            // sort stops by ascending StopID
            Array.Sort(Stops, new Comparers.ComparerForStopSorting());

            // exploit similar ordering between MetroNodes and Stops to associate Stops with MetroNodes in one pass
            int MetroNodeCounter = 0;

            StopToNearest = new Dictionary<int, List<int>>();
            StopToNodes = new Dictionary<int, List<IMetroNode>>();

            foreach (var Stop1 in Stops)
            {
                // associate Id's of closest stops with this stop
                StopToNearest.Add(Stop1.Id, Stop1.CloseStops.Select(s => s.Id).ToList());

                // associate MetroNodes which contain this stop with this stop
                List<IMetroNode> Stop1NodeList = new List<IMetroNode>();

                // in case some metronodes need to be skipped
                while (MetroNodeCounter < MetroNodes.Count()
                       && MetroNodes[MetroNodeCounter].StopID < Stop1.Id)
                {
                    MetroNodeCounter++;
                }

                while (MetroNodeCounter < MetroNodes.Count()
                       && MetroNodes[MetroNodeCounter].StopID == Stop1.Id)
                {
                    Stop1NodeList.Add(MetroNodes[MetroNodeCounter]);
                    MetroNodeCounter++;
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

        private INode[] InsertChildCareNodes(IEnumerable<Stop> Stops, IEnumerable<ChildCare> ChildCares, INode[] GraphNodes)
        {
            if (Stops == null || ChildCares == null || GraphNodes == null)
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

                var StopEnumerator = Stops.GetEnumerator();

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
