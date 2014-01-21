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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private GraphBuilderSettings _settings;
        private Dictionary<int, List<IGtfsNode>> _stopToNodes;

        public GraphBuilder() 
        {
            Logger.Trace("GraphBuilder object created.");
        }

        public IGraph BuildGraph(IEnumerable<StopTime> stopTimes, IEnumerable<IDestination> destinations, GraphBuilderSettings settings)
        {
            _settings = settings;

            Logger.Trace("Creating new graph.");

            // enumerate the stop times
            StopTime[] stopTimeArray = stopTimes.ToArray();

            // enumerate the destinations
            IDestination[] destinationArray = destinations.ToArray();

            // collect all the stops
            Stop[] stops = stopTimeArray
                .GroupBy(s => s.StopId)
                .Select(g => g.First().Stop)
                .ToArray();

            var gtfsNodes = CreateGtfsNodes(stopTimeArray);
            ConnectTrips(gtfsNodes);
            ConnectTransfers(gtfsNodes, stops);
            var graphNodes = InsertDestinationNodes(stops, destinationArray, gtfsNodes);

            var graph = new Graph(stops, _stopToNodes, graphNodes, _settings);

            Logger.Trace("Graph created successfully.");

            return graph;
        }

        private IGtfsNode[] CreateGtfsNodes(IEnumerable<StopTime> stopTimes)
        {
            Logger.Trace("Creating GTFS Nodes.");

            IGtfsNode[] gtfsNodes = stopTimes
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

                if (node.TripId == previousNode.TripId)
                {
                    node.TimeBackwardNeighbors.Add(previousNode);
                    previousNode.TimeForwardNeighbors.Add(node);
                }
            }
            Logger.Trace("GTFS Trips connected successfully.");
        }

        private void ConnectTransfers(IGtfsNode[] gtfsNodes, Stop[] stops)
        {
            Logger.Trace("Connecting GTFS Transfers.");

            // sorts Nodes first by ascending StopId, second by ascending Time
            Array.Sort(gtfsNodes, new Comparers.ComparerForTransferSorting());

            // sort stops by ascending StopId
            Array.Sort(stops, new Comparers.ComparerForStopSorting());

            // exploit similar ordering between gtfsNodes and Stops to associate Stops with gtfsNodes in one pass
            int gtfsNodeCounter = 0;

            // calculate the close stops
            var stopToNearest = stops
                .Select(stopA => new
                {
                    StopId = stopA.Id,
                    ClosestStopIds = stops
                        .Where(stopB => stopA.GetL1DistanceInFeet(stopB) < _settings.MaxFeetBetweenTransfers)
                        .Select(stopB => stopB.Id).ToList()
                })
                .ToDictionary(t => t.StopId, t => t.ClosestStopIds);

            _stopToNodes = new Dictionary<int, List<IGtfsNode>>();

            foreach (var stop in stops)
            {
                // associate gtfsNodes which contain this stop with this stop
                var stopNodeList = new List<IGtfsNode>();

                // in case some GTFS nodes need to be skipped
                while (gtfsNodeCounter < gtfsNodes.Count()
                       && gtfsNodes[gtfsNodeCounter].StopId < stop.Id)
                {
                    gtfsNodeCounter++;
                }

                while (gtfsNodeCounter < gtfsNodes.Count()
                       && gtfsNodes[gtfsNodeCounter].StopId == stop.Id)
                {
                    stopNodeList.Add(gtfsNodes[gtfsNodeCounter]);
                    gtfsNodeCounter++;
                }

                _stopToNodes.Add(stop.Id, stopNodeList);
            }

            // loop through gtfsNodes and connect transfers
            foreach (var node1 in gtfsNodes)
            {
                // obtain StopId's of nodes in transfer distance
                List<int> nearestIds;
                if (!stopToNearest.TryGetValue(node1.StopId, out nearestIds))
                {
                    continue;
                }

                foreach (var id in nearestIds)
                {
                    List<IGtfsNode> nodes;
                    if (!_stopToNodes.TryGetValue(id, out nodes))
                    {
                        continue;
                    }

                    if (nodes.Count == 0) continue; // not sure if this is possible

                    // calculate walking time, will be same for all nodes in list, so use first
                    double walkingTime = node1.GetL1DistanceInFeet(nodes.First()) / _settings.WalkingFeetPerSecond;
                    DateTime minTime = node1.Time + new TimeSpan(0, 0, (int)Math.Ceiling(walkingTime));

                    // thanks to sorting, these nodes are iterated in ascending time
                    foreach (var node2 in nodes)
                    {
                        // don't connect same stop on same trip (why just sit there?)
                        if (node1.BaseNode == node2.BaseNode) continue;

                        if (node2.Time > minTime)
                        {
                            node1.TimeForwardNeighbors.Add(node2);
                            node2.TimeBackwardNeighbors.Add(node1);
                            break;
                        }
                    }
                }
            }

            Logger.Trace("GTFS Transfers connected successfully.");
        }

        private IEnumerable<INode> InsertDestinationNodes(Stop[] stops, IDestination[] destinations, IEnumerable<INode> graphNodes)
        {
            if (stops == null || destinations == null || graphNodes == null)
            {
                throw new ArgumentNullException();
            }

            var graphNodeList = graphNodes.ToList();

            // associate destinations with StopId's of closest stops
            var destinationToStops = new Dictionary<IDestination, List<int>>();

            foreach (var destination in destinations)
            {
                var nearestStopIDs = new int[_settings.MaxDestinationCloseStops];
                for (int i = 0; i < _settings.MaxDestinationCloseStops; i++)
                {
                    nearestStopIDs[i] = int.MinValue;
                }
                var nearestDistances = new double[_settings.MaxDestinationCloseStops];
                for (int i = 0; i < _settings.MaxDestinationCloseStops; i++)
                {
                    nearestDistances[i] = double.MaxValue;
                }
                
                foreach (var stop in stops)
                {
                    double distance = destination.GetL1DistanceInFeet(stop);

                    if (distance < _settings.MaxFeetFromDestinationToStop)
                    {
                        // store the closest stops to this destination
                        var stopId = stop.Id;
                        for (int i = 0; i < _settings.MaxDestinationCloseStops; i++)
                        {
                            if (distance < nearestDistances[i])
                            {
                                int tempId = stopId;
                                double tempDistance = distance;
                                stopId = nearestStopIDs[i];
                                distance = nearestDistances[i];
                                nearestStopIDs[i] = tempId;
                                nearestDistances[i] = tempDistance;
                            }
                        }
                    }
                }

                var nearestStopList = nearestStopIDs.ToList();
                nearestStopList.RemoveAll(x => x == int.MinValue);
                
                if (nearestStopList.Any())
                {
                    destinationToStops.Add(destination, nearestStopList);
                }
            }

            // now for each destination we insert many nodes into the graph

            foreach (var destination in destinations)
            {
                List<int> nearestStops;
                if (!destinationToStops.TryGetValue(destination, out nearestStops))
                {
                    continue;
                }

                // maintain list of destination nodes added, these nodes will be
                // connected to each other, and this connection will correspond to waiting
                // at that destination between two specific times
                var destinationNodesAdded = new List<IDestinationNode>();

                // give each destination node the same base node
                var baseNode = new NodeBase(destination.Name, destination.Latitude, destination.Longitude);

                foreach (var stop in nearestStops)
                {
                    // retrieve nodes that have to be connected to
                    List<IGtfsNode> nodes;
                    if (!_stopToNodes.TryGetValue(stop, out nodes))
                    {
                        continue;
                    }

                    // make sure we have some nodes to work with
                    if (nodes.Count == 0) continue;

                    var distance = destination.GetL1DistanceInFeet(nodes.First());
                    var walkingTime = TimeSpan.FromSeconds(distance / _settings.WalkingFeetPerSecond);

                    // for each GTFS node, create upwind / downwind destination nodes and connect them
                    foreach (var node in nodes)
                    {
                        var forwardTime = node.Time + walkingTime;
                        var backwardTime = node.Time - walkingTime;

                        var forwardDestinationNode = new DestinationNode(destination, forwardTime, baseNode);
                        var backwardDestinationNode = new DestinationNode(destination, backwardTime, baseNode);

                        node.TimeBackwardNeighbors.Add(backwardDestinationNode);
                        node.TimeForwardNeighbors.Add(forwardDestinationNode);
                        forwardDestinationNode.TimeBackwardNeighbors.Add(node);
                        backwardDestinationNode.TimeForwardNeighbors.Add(node);

                        destinationNodesAdded.Add(forwardDestinationNode);
                        destinationNodesAdded.Add(backwardDestinationNode);
                    }
                }

                // add new destination nodes to list of Graph nodes
                graphNodeList.AddRange(destinationNodesAdded);

                // sort destination nodes by ascending time
                var destinationNodesArray = destinationNodesAdded.ToArray();
                Array.Sort(destinationNodesArray, new Comparers.ComparerForDestinations());

                for (int i = 1; i < destinationNodesArray.Count(); i++)
                {
                    var current = destinationNodesArray[i];
                    var previous = destinationNodesArray[i - 1];

                    current.TimeBackwardNeighbors.Add(previous);
                    previous.TimeForwardNeighbors.Add(current);
                }
            }

            return graphNodeList.ToArray();
        }
    }
}
