using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using NLog;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Graph
{
    public class GraphBuilder : IGraphBuilder
    {
        private readonly IGtfsNode _gtfsNodeMaker;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public GraphBuilderSettings Settings { get; private set; }
        public Dictionary<int, List<IGtfsNode>> StopToNodes { get; private set; } // from StopID to set of Nodes with given StopID

        public GraphBuilder(IGtfsNode gtfsNodeMaker) 
        {
            _gtfsNodeMaker = gtfsNodeMaker;
            Logger.Trace("GraphBuilder object created.");
        }

        public INode[] BuildGraph(IEnumerable<StopTime> StopTimes, IEnumerable<Destination> Destinations, GraphBuilderSettings Settings)
        {
            this.Settings = Settings;

            Logger.Trace("Creating new graph.");

            // collect all the stops
            Stop[] Stops = StopTimes
                .GroupBy(s => s.StopId)
                .Select(g => g.First().Stop)
                .ToArray();

            var MetroNodes = CreateGtfsNodes(Stops, StopTimes);
            ConnectTrips(MetroNodes);
            ConnectTransfers(MetroNodes, Stops);
            var GraphNodes = InsertDestinationNodes(Stops, Destinations, MetroNodes);
            Logger.Trace("Graph created successfully.");
            return GraphNodes;
        }

        private IGtfsNode[] CreateGtfsNodes(IEnumerable<Stop> Stops, IEnumerable<StopTime> StopTimes)
        {
            Logger.Trace("Creating GTFS Nodes.");

            IGtfsNode[] gtfsNodes = StopTimes
                .GroupBy(s => new Tuple<int, int, int?, int>(s.StopId, s.Trip.RouteId, s.Trip.ShapeId, s.Sequence))
                .SelectMany(g =>
                {
                    StopTime first = g.First();
                    var nodeBase = new NodeBase(
                        first.Stop.Name,
                        first.Stop.Latitude,
                        first.Stop.Longitude);

                    return g.Select(s => (IGtfsNode) new GtfsNode(s, nodeBase));
                })
                .ToArray();

            Logger.Trace("GTFS Nodes created successfully.");

            return gtfsNodes;
        }

        private void ConnectTrips(IGtfsNode[] gtfsNodes)
        {
            Logger.Trace("Connecting GTFS Trips.");

            Array.Sort(gtfsNodes, new Comparers.ComparerForTripSorting());

            for (int i = 1; i < gtfsNodes.Count(); i++)
            {
                IGtfsNode node = gtfsNodes[i];
                IGtfsNode previousNode = gtfsNodes[i-1];

                if (node.TripID == previousNode.TripID)
                {
                    node.TimeBackwardNeighbors.Add(previousNode);
                    previousNode.TimeForwardNeighbors.Add(node);
                }
            }
            Logger.Trace("Metro Trips connected successfully.");
        }

        private void ConnectTransfers(IGtfsNode[] gtfsNodes, Stop[] Stops)
        {
            Logger.Trace("Connecting GTFS Transfers.");

            // sorts Nodes first by ascending stopID, second by ascending Time
            Array.Sort(gtfsNodes, new Comparers.ComparerForTransferSorting());

            // sort stops by ascending StopID
            Array.Sort(Stops, new Comparers.ComparerForStopSorting());

            // exploit similar ordering between gtfsNodes and Stops to associate Stops with gtfsNodes in one pass
            int MetroNodeCounter = 0;

            // calculate the close stops
            var stopToNearest = Stops
                .Select(stopA => new
                {
                    StopId = stopA.Id,
                    ClosestStopIds = Stops
                        .Where(stopB => stopA.GetL1DistanceInFeet(stopB) < Settings.MaxFeetBetweenTransfers)
                        .Select(stopB => stopB.Id).ToList()
                })
                .ToDictionary(t => t.StopId, t => t.ClosestStopIds);

            StopToNodes = new Dictionary<int, List<IGtfsNode>>();

            foreach (var stop in Stops)
            {
                // associate gtfsNodes which contain this stop with this stop
                List<IGtfsNode> stopNodeList = new List<IGtfsNode>();

                // in case some metronodes need to be skipped
                while (MetroNodeCounter < gtfsNodes.Count()
                       && gtfsNodes[MetroNodeCounter].StopID < stop.Id)
                {
                    MetroNodeCounter++;
                }

                while (MetroNodeCounter < gtfsNodes.Count()
                       && gtfsNodes[MetroNodeCounter].StopID == stop.Id)
                {
                    stopNodeList.Add(gtfsNodes[MetroNodeCounter]);
                    MetroNodeCounter++;
                }

                StopToNodes.Add(stop.Id, stopNodeList);
            }

            // loop through gtfsNodes and connect transfers
            foreach (var node1 in gtfsNodes)
            {
                // obtain stopID's of nodes in transfer distance
                List<int> NearestIDs = null;
                if (!stopToNearest.TryGetValue(node1.StopID, out NearestIDs))
                {
                    continue;
                }

                foreach (var ID in NearestIDs)
                {
                    List<IGtfsNode> Nodes = null;
                    if (!StopToNodes.TryGetValue(ID, out Nodes))
                    {
                        continue;
                    }

                    if (Nodes.Count == 0) continue; // not sure if this is possible

                    // calculate walking time, will be same for all nodes in list, so use first
                    double WalkingTime = node1.GetL1DistanceInFeet(Nodes.First()) / Settings.WalkingFeetPerSecond;
                    DateTime MinTime = node1.Time + new TimeSpan(0, 0, (int)Math.Ceiling(WalkingTime));

                    // thanks to sorting, these nodes are iterated in ascending time
                    foreach (var node2 in Nodes)
                    {
                        // don't connect same stop on same trip (why just sit there?)
                        if (node1.BaseNode == node2.BaseNode) continue;

                        if (node2.Time > MinTime)
                        {
                            node1.TimeForwardNeighbors.Add(node2);
                            node2.TimeBackwardNeighbors.Add(node1);
                            break;
                        }
                    }
                }
            }

            Logger.Trace("Metro Transfers connected successfully.");
        }

        private INode[] InsertDestinationNodes(IEnumerable<Stop> Stops, IEnumerable<Destination> Destinations, INode[] GraphNodes)
        {
            if (Stops == null || Destinations == null || GraphNodes == null)
            {
                throw new ArgumentNullException();
            }

            var GraphNodeList = GraphNodes.ToList();

            // associate ChildCares with StopID's of closest stops
            var destinationToStops = new Dictionary<int, List<int>>();

            foreach (var destination in Destinations)
            {
                var nearestStopIDs = new int[Settings.MaxDestinationCloseStops];
                for (int i = 0; i < Settings.MaxDestinationCloseStops; i++) nearestStopIDs[i] = int.MinValue;
                var nearestDistances = new double[Settings.MaxDestinationCloseStops];
                for (int i = 0; i < Settings.MaxDestinationCloseStops; i++) nearestDistances[i] = double.MaxValue;
                
                foreach (var stop in Stops)
                {
                    double distance = destination.GetL1DistanceInFeet(stop);

                    if (distance < Settings.MaxFeetFromChildCareToBuStop)
                    {
                        // store the closest stops to this childcare
                        var StopID = stop.Id;
                        for (int i = 0; i < Settings.MaxDestinationCloseStops; i++)
                        {
                            if (distance < nearestDistances[i])
                            {
                                var tempID = StopID; var tempDistance = distance;
                                StopID = nearestStopIDs[i];
                                distance = nearestDistances[i];
                                nearestStopIDs[i] = tempID;
                                nearestDistances[i] = tempDistance;
                            }
                        }
                    }
                }

                var nearestStopList = nearestStopIDs.ToList();
                nearestStopList.RemoveAll(x => x == int.MinValue);
                
                if (nearestStopList.Count() > 0)
                {
                    destinationToStops.Add(destination.Id, nearestStopList);
                }
            }

            // now for each ChildCare we insert many nodes into the graph

            foreach (var destination in Destinations)
            {
                List<int> nearestStops = null;

                if (!destinationToStops.TryGetValue(destination.Id, out nearestStops))
                {
                    continue;
                }

                // maintain list of childcare nodes added, these nodes will be
                // connected to each other, and this connection will correspond to waiting
                // at that childcare between two specific times
                var ChildCareNodesAdded = new List<IDestinationNode>();

                // give each child care node the same base node
                var BaseNode = new NodeBase(destination.Name, destination.Latitude, destination.Longitude);

                foreach (var stop in nearestStops)
                {
                    // retrieve nodes that have to be connected to
                    List<IGtfsNode> nodes = null;
                    if (!StopToNodes.TryGetValue(stop, out nodes))
                    {
                        continue;
                    }

                    // make sure we have some nodes to work with
                    if (nodes.Count == 0) continue;

                    var distance = destination.GetL1DistanceInFeet(nodes.First());
                    var walkingTime = TimeSpan.FromSeconds(distance / Settings.WalkingFeetPerSecond);

                    // for each metro node, create upwind / downwind ChildCare nodes and connect them
                    foreach (var node in nodes)
                    {
                        var forwardTime = node.Time + walkingTime;
                        var backwardTime = node.Time - walkingTime;

                        var ForwardChildCareNode = new DestinationNode(destination, forwardTime, BaseNode);
                        var BackwardChildCareNode = new DestinationNode(destination, backwardTime, BaseNode);

                        node.TimeBackwardNeighbors.Add(BackwardChildCareNode);
                        node.TimeForwardNeighbors.Add(ForwardChildCareNode);
                        ForwardChildCareNode.TimeBackwardNeighbors.Add(node);
                        BackwardChildCareNode.TimeForwardNeighbors.Add(node);

                        ChildCareNodesAdded.Add(ForwardChildCareNode);
                        ChildCareNodesAdded.Add(BackwardChildCareNode);
                    }
                }

                // add new ChildCare nodes to list of Graph nodes
                GraphNodeList.AddRange(ChildCareNodesAdded);

                // sort child care nodes by ascending time
                var ChildCareNodesArray = ChildCareNodesAdded.ToArray();
                Array.Sort(ChildCareNodesArray, new Comparers.ComparerForDestinations());

                for (int i = 1; i < ChildCareNodesArray.Count(); i++)
                {
                    var current = ChildCareNodesArray[i];
                    var previous = ChildCareNodesArray[i - 1];

                    current.TimeBackwardNeighbors.Add(previous);
                    previous.TimeForwardNeighbors.Add(current);
                }
            }

            return GraphNodeList.ToArray();
        }
    }
}
