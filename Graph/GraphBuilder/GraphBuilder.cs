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
        public GraphBuilderSettings Settings { get; private set; }
        public Dictionary<int, List<int>> StopToNearest { get; private set; } // from StopID to list of StopID's of nearest Stops
        public Dictionary<int, List<IMetroNode>> StopToNodes { get; private set; } // from StopID to set of Nodes with given StopID
        public Dictionary<int, List<int>> ChildCareToStops { get; private set; } // from ChildCare ID to closest metro stops

        public GraphBuilder(IMetroNode metroNodeMaker) 
        {
            _metroNodeMaker = metroNodeMaker;
            Logger.Trace("GraphBuilder object created.");
        }

        public INode[] BuildGraph(IEnumerable<StopTime> StopTimes, IEnumerable<ChildCare> ChildCares)
        {
            Logger.Trace("Creating new graph.");

            Settings = GraphBuilderSettings.Default;

            // collect all the stops
            Stop[] Stops = StopTimes
                .GroupBy(s => s.StopId)
                .Select(g => g.First().Stop)
                .ToArray();

            var MetroNodes = CreateMetroNodes(Stops, StopTimes);
            ConnectTrips(MetroNodes);
            ConnectTransfers(MetroNodes, Stops);
            var GraphNodes = InsertChildCareNodes(Stops, ChildCares, MetroNodes);
            Logger.Trace("Graph created successfully.");
            return GraphNodes;
        }

        public INode[] BuildGraph(IEnumerable<StopTime> StopTimes, IEnumerable<ChildCare> ChildCares, GraphBuilderSettings Settings)
        {
            this.Settings = Settings;
            return BuildGraph(StopTimes, ChildCares);
        }

        private IMetroNode[] CreateMetroNodes(IEnumerable<Stop> Stops, IEnumerable<StopTime> StopTimes)
        {
            Logger.Trace("Creating Metro Nodes.");

            // sort StopTimes by TripID first, StopID second
            var StopTimesArray = StopTimes.ToArray();
            Array.Sort(StopTimesArray, new Comparers.ComparerForStopTimeSorting());

            var MetroNodeList = new List<IMetroNode>();
            int counter = 0;

            // nodes with same StopID and same TripID reference the same NodeBase
            while (counter < StopTimesArray.Count())
            {
                var stoptime = StopTimesArray[counter];
                int CurrentStopID = stoptime.StopId;
                int CurrentShapeID = stoptime.Trip.ShapeId.Value;
                
                var BaseNode = new NodeBase(
                    String.Concat(new string[] { stoptime.Stop.Name, String.Format(" <ShapeID {0}>", stoptime.Trip.ShapeId.Value) }), 
                    stoptime.Stop.Latitude, 
                    stoptime.Stop.Longitude);

                while (counter < StopTimesArray.Count()
                    && StopTimesArray[counter].StopId == CurrentStopID
                    && StopTimesArray[counter].Trip.ShapeId.Value == CurrentShapeID)
                {
                    MetroNodeList.Add(new MetroNode(StopTimesArray[counter], BaseNode));
                    counter++;
                }
            }

            Logger.Trace("Metro Nodes created successfully.");
            return MetroNodeList.ToArray();
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
                    node.TimeBackwardNeighbors.Add(previousNode);
                    previousNode.TimeForwardNeighbors.Add(node);
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

            foreach (var stop in Stops)
            {
                // associate Id's of closest stops with this stop
                StopToNearest.Add(stop.Id, stop.CloseStops.Select(s => s.Id).ToList());

                // associate MetroNodes which contain this stop with this stop
                List<IMetroNode> stopNodeList = new List<IMetroNode>();

                // in case some metronodes need to be skipped
                while (MetroNodeCounter < MetroNodes.Count()
                       && MetroNodes[MetroNodeCounter].StopID < stop.Id)
                {
                    MetroNodeCounter++;
                }

                while (MetroNodeCounter < MetroNodes.Count()
                       && MetroNodes[MetroNodeCounter].StopID == stop.Id)
                {
                    stopNodeList.Add(MetroNodes[MetroNodeCounter]);
                    MetroNodeCounter++;
                }

                StopToNodes.Add(stop.Id, stopNodeList);
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

        private INode[] InsertChildCareNodes(IEnumerable<Stop> Stops, IEnumerable<ChildCare> ChildCares, INode[] GraphNodes)
        {
            if (Stops == null || ChildCares == null || GraphNodes == null)
            {
                throw new ArgumentNullException();
            }

            var GraphNodeList = GraphNodes.ToList();

            // associate ChildCares with StopID's of closest stops
            ChildCareToStops = new Dictionary<int, List<int>>();

            foreach(var childcare in ChildCares)
            {
                // make sure lat/long are not null
                if (!childcare.Latitude.HasValue || !childcare.Longitude.HasValue)
                {
                    continue;
                }

                var nearestStopIDs = new int[Settings.MaxChildCareCloseStops];
                for (int i = 0; i < Settings.MaxChildCareCloseStops; i++) nearestStopIDs[i] = int.MinValue;
                var nearestDistances = new double[Settings.MaxChildCareCloseStops];
                for (int i = 0; i < Settings.MaxChildCareCloseStops; i++) nearestDistances[i] = double.MaxValue;
                
                foreach (var stop in Stops)
                {
                    double distance = childcare.GetL1DistanceInFeet(stop);

                    if (distance < Settings.MaxFeetFromChildCareToBuStop)
                    {
                        // store the closest stops to this childcare
                        var StopID = stop.Id;
                        for (int i = 0; i < Settings.MaxChildCareCloseStops; i++)
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
                    ChildCareToStops.Add(childcare.Id, nearestStopList);
                }
            }

            // now for each ChildCare we insert many nodes into the graph

            foreach (var childcare in ChildCares)
            {
                List<int> nearestStops = null;

                if (!ChildCareToStops.TryGetValue(childcare.Id, out nearestStops))
                {
                    continue;
                }

                if (!childcare.Latitude.HasValue || !childcare.Longitude.HasValue)
                {
                    continue;
                }

                // maintain list of childcare nodes added, these nodes will be
                // connected to each other, and this connection will correspond to waiting
                // at that childcare between two specific times
                var ChildCareNodesAdded = new List<IChildcareNode>();

                // create base node which all ChildCare nodes for this ChildCare whill reference
                var BaseNode = new NodeBase(childcare.Name, childcare.Latitude.Value, childcare.Longitude.Value);

                foreach (var stop in nearestStops)
                {
                    // retrieve nodes that have to be connected to
                    List<IMetroNode> nodes = null;
                    if (!StopToNodes.TryGetValue(stop, out nodes))
                    {
                        continue;
                    }

                    // make sure we have some nodes to work with
                    if (nodes.Count == 0) continue;

                    var distance = childcare.GetL1DistanceInFeet(nodes.First());
                    var walkingTime = TimeSpan.FromSeconds(distance / Settings.WalkingFeetPerSecond);

                    // for each metro node, create upwind / downwind ChildCare nodes and connect them
                    foreach (var node in nodes)
                    {
                        var forwardTime = node.Time + walkingTime;
                        var backwardTime = node.Time - walkingTime;

                        var ForwardChildCareNode = new ChildCareNode(childcare, forwardTime, BaseNode);
                        var BackwardChildCareNode = new ChildCareNode(childcare, backwardTime, BaseNode);

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
                    
                // now connect child care nodes together. Fist sort by time
                //var ChildCareNodesArray = ChildCareNodesAdded.ToArray();
                //Array.Sort(ChildCareNodesArray, new Comparers.ComparerForChildCares());

                //for (int i = 0; i < ChildCareNodesArray.Count() - 1; i++)
                //{
                //    for (int j = i + 1; j < ChildCareNodesArray.Count(); j++)
                //    {
                //        var previousChildCareNode = ChildCareNodesArray[i];
                //        var currentChildCareNode = ChildCareNodesArray[j];

                //        if (previousChildCareNode.BaseNode != currentChildCareNode.BaseNode)
                //        {
                //            previousChildCareNode.TimeForwardNeighbors.Add(currentChildCareNode);
                //            currentChildCareNode.TimeBackwardNeighbors.Add(previousChildCareNode);
                //            break;
                //        }
                //    }
                //}
            }

            return GraphNodeList.ToArray();
        }
    }
}
