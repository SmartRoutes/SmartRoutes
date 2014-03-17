using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Graph.Heap;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
    public class Graph : IGraph
    {
        private readonly Stop[] _stops;
        public INode[] GraphNodes { get; private set; }
        private readonly GraphBuilderSettings _settings;
        private readonly IDictionary<int, List<IGtfsNode>> _stopToNodes;

        public Graph(IEnumerable<Stop> stops, IDictionary<int, List<IGtfsNode>> stopToNodes, IEnumerable<INode> nodes, GraphBuilderSettings settings)
        {
            _stops = stops.ToArray();
            _stopToNodes = stopToNodes;
            GraphNodes = nodes.ToArray();
            _settings = settings;
        }

        private IEnumerable<IGtfsNode> GetClosestGtfsNodes(ILocation location, DateTime time, TimeDirection direction)
        {
            Func<Stop, double> stopDistanceOrdering = s => s.GetL1DistanceInFeet(location);
            Func<Stop, bool> stopDistanceFilter = s => s.GetL1DistanceInFeet(location) < _settings.MaxFeetFromDestinationToStop;

            Func<IGtfsNode, TimeSpan> walkingTime = n =>
            {
                double distance = n.GetL1DistanceInFeet(location);
                return TimeSpan.FromSeconds(distance / _settings.WalkingFeetPerSecond);
            };

            Func<IGtfsNode, bool> nodeTimeFilter;

            if (direction == TimeDirection.Forwards)
            {
                nodeTimeFilter = n => n.Time - walkingTime(n) >= time;
            }
            else
            {
                nodeTimeFilter = n => n.Time + walkingTime(n) <= time;
            }

            var closeStops = _stops.Where(stopDistanceFilter).OrderBy(stopDistanceOrdering);

            var closeNodes = closeStops.SelectMany(s =>
            {
                List<IGtfsNode> relatedNodes = null;
                _stopToNodes.TryGetValue(s.Id, out relatedNodes);
                return relatedNodes;
            }).Where(nodeTimeFilter);

            IEnumerable<IGtfsNode> orderedCloseNodes;
            if (direction == TimeDirection.Forwards)
            {
                orderedCloseNodes = closeNodes.OrderBy(n => n.Time + walkingTime(n));
            }
            else
            {
                orderedCloseNodes = closeNodes.OrderByDescending(n => n.Time - walkingTime(n));
            }

            // return the closest node from each (RouteId, BlockId) group
            var returnVal = orderedCloseNodes.GroupBy(n => new Tuple<int, int?>(n.RouteId, n.BlockId)).Select(g => g.First());
            return orderedCloseNodes
                .GroupBy(node => new Tuple<NodeBase>(node.BaseNode))
                .Select(g => g.First());
        }

        private IEnumerable<NodeInfo> GetClosestGtfsNodeInfos(ILocation location, DateTime time, TimeDirection direction, TimeSpan basePathCost)
        {
            var closestGtfsNodes = GetClosestGtfsNodes(location, time, direction);

            return closestGtfsNodes.Select(node =>
                {
                    TimeSpan walkingTime = TimeSpan.FromSeconds(
                        location.GetL1DistanceInFeet(node) / _settings.WalkingFeetPerSecond);
                    var info = new NodeInfo();
                    info.Node = node;
                    info.PathCost = (direction == TimeDirection.Forwards)
                        ? node.Time - time
                        : time - node.Time;
                    info.PathCost += basePathCost + walkingTime;
                    info.State = NodeState.Open;
                    return info;
                });
        }

        private static TimeSpan GetNextPathCost(NodeInfo currentInfo, INode neighbor, TimeDirection direction)
        {
            TimeSpan travelTimeBetween = neighbor.Time - currentInfo.Node.Time;
            TimeSpan pathCost = (direction == TimeDirection.Forwards)
                ? currentInfo.PathCost + travelTimeBetween
                : currentInfo.PathCost - travelTimeBetween;

            // penalize transfers
            var gtfsCurrent = currentInfo.Node as IGtfsNode;
            var gtfsNeighbor = neighbor as IGtfsNode;
            if (gtfsCurrent != null && gtfsNeighbor != null)
            {
                if (gtfsCurrent.TripId != gtfsNeighbor.TripId)
                {
                    pathCost += TimeSpan.FromMinutes(5);
                }
            }

            return pathCost;
        }

        private IEnumerable<NodeInfo> Dijkstras(IEnumerable<NodeInfo> StartNodeInfos, Func<INode, bool> GoalCheck, TimeDirection direction, ILocation EndLocation = null)
        {
            var Results = new List<NodeInfo>();

            if (StartNodeInfos.Count() == 0) return Results;

            var SearchInfo = new Dictionary<NodeBase, NodeInfo>();
            var heap = new FibonacciHeap<NodeBase, TimeSpan>();

            // assign search info to StartNodes and place them in queue
            foreach (var nodeInfo in StartNodeInfos)
            {
                nodeInfo.Handle = heap.Insert(nodeInfo.Node.BaseNode, nodeInfo.PathCost);
                SearchInfo.Add(nodeInfo.Node.BaseNode, nodeInfo);
            }

            while (!heap.Empty())
            {
                NodeBase currentBase = heap.DeleteMin();

                // get search info
                NodeInfo currentInfo = null;
                if (!SearchInfo.TryGetValue(currentBase, out currentInfo))
                {
                    throw new KeyNotFoundException("Node removed from heap did not have associated search info: ");
                }

                var current = currentInfo.Node;

                // check for completion
                if (GoalCheck(current))
                {
                    Results.Add(currentInfo);
                    if (Results.Count() > 30) break;
                    currentInfo.State = NodeState.Closed;
                    continue;
                }
                else
                {
                    // if this is destination, no need to visit, otherwise search
                    // cuts through destinations to avoid transfer pathcost penalty
                    if (current as DestinationNode != null) continue;
                }

                // if looking for end location, check if current is close enough and if so
                // add a goal node to heap connecting to current
                if (EndLocation != null && current.GetL1DistanceInFeet(EndLocation) < _settings.MaxFeetFromDestinationToStop)
                {
                    TimeSpan walkingTime = TimeSpan.FromSeconds(
                        current.GetL1DistanceInFeet(EndLocation) / _settings.WalkingFeetPerSecond);
                    DateTime goalTime = (direction == TimeDirection.Forwards)
                        ? current.Time + walkingTime
                        : current.Time - walkingTime;
                    var GoalInfo = new NodeInfo();
                    GoalInfo.Node = new LocationGoalNode(goalTime);
                    GoalInfo.Parent = currentInfo;
                    GoalInfo.PathCost = currentInfo.PathCost + walkingTime;
                    GoalInfo.Handle = heap.Insert(GoalInfo.Node.BaseNode, GoalInfo.PathCost);
                    SearchInfo.Add(GoalInfo.Node.BaseNode, GoalInfo);
                }

                // loop through neighbors and handle business
                var Neighbors = (direction == TimeDirection.Forwards) ?
                    current.TimeForwardNeighbors : current.TimeBackwardNeighbors;

                foreach (var neighbor in Neighbors)
                {
                    if (neighbor.BaseNode == current.BaseNode) continue;

                    NodeInfo neighborInfo = null;
                    if (!SearchInfo.TryGetValue(neighbor.BaseNode, out neighborInfo))
                    {
                        // node is new, give it search info and place in queue
                        neighborInfo = new NodeInfo();
                        neighborInfo.Node = neighbor;
                        neighborInfo.Parent = currentInfo;
                        neighborInfo.State = NodeState.Open;
                        neighborInfo.PathCost = GetNextPathCost(currentInfo, neighbor, direction);
                        neighborInfo.Handle = heap.Insert(neighbor.BaseNode, neighborInfo.PathCost);
                        SearchInfo.Add(neighbor.BaseNode, neighborInfo);
                    }
                    else
                    {
                        // neighbor is in queue, check state
                        if (neighborInfo.State == NodeState.Open)
                        {
                            // update neighborInfo if this route is better
                            TimeSpan newPathCost = GetNextPathCost(currentInfo, neighbor, direction);
                            if (newPathCost < neighborInfo.PathCost)
                            {
                                // update search info and update queue for new key
                                neighborInfo.PathCost = newPathCost;
                                neighborInfo.Parent = currentInfo;
                                neighborInfo.Node = neighbor;
                                heap.UpdateKey(neighborInfo.Handle, newPathCost);
                            }
                        }
                    }
                }

                // and we're done with current
                currentInfo.State = NodeState.Closed;
            }

            // return only one result per destination
            var uniqueDestinationResults = Results.Where(info => info.Node as DestinationNode != null)
                .GroupBy(info => new Tuple<IDestination>(((DestinationNode)info.Node).Destination))
                .Select(g => g.First());

            // return only one result per block (set of sequential trips)
            var uniqueBlockResults = Results.Where(info => info.Node as LocationGoalNode != null && info.Parent.Node as IGtfsNode != null)
                .GroupBy(info => new Tuple<int, int?>(((IGtfsNode)info.Parent.Node).TripId, ((IGtfsNode)info.Parent.Node).BlockId))
                .Select(g => g.First());

            return uniqueBlockResults.Concat(uniqueDestinationResults);
        }

        private NodeInfo ConcatResults(NodeInfo A, NodeInfo B, TimeDirection Direction)
        {
            if (A.Node.Time < B.Node.Time) return ConcatResults(B, A, Direction);

            if (Direction == TimeDirection.Forwards)
            {
                var current = A;
                while (current.Parent != null) current = current.Parent;
                if (current.Node.Time < B.Node.Time) throw new Exception("Cannot concat results: overlapping times");
                current.Parent = B;
                return A;
            }
            else
            {
                var current = B;
                while (current.Parent != null) current = current.Parent;
                if (current.Node.Time > A.Node.Time) throw new Exception("Cannot concat results: overlapping times");
                current.Parent = A;
                return B;
            }
        }

        // creates goal check function which checks whether a node satisfies at least one criteria
        private Func<INode, bool> CreateGoalCheckFromCriterion(IEnumerable<Func<IDestination, bool>> Criterion)
        {
            Func<INode, bool> GoalCheck = node =>
                {
                    var destNode = node as DestinationNode;
                    if (destNode != null)
                    {
                        var RemainingCriterion = Criterion.Where(F => !F(destNode.Destination));

                        if (RemainingCriterion.Count() != Criterion.Count())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                };

            return GoalCheck;
        }

        private IEnumerable<NodeInfo> SearchLocToDest(ILocation StartLocation, DateTime StartTime, TimeDirection Direction, 
            IEnumerable<Func<IDestination, bool>> Criterion, TimeSpan BasePathCost)
        {
            var FinalResults = new List<NodeInfo>();
            var StartNodeInfos = GetClosestGtfsNodeInfos(StartLocation, StartTime, Direction, BasePathCost);

            var partialResults = Dijkstras(StartNodeInfos, CreateGoalCheckFromCriterion(Criterion), Direction);

            if (partialResults.Count() == 0) return FinalResults;

            foreach (var partialResult in partialResults)
            {
                var RemainingCriterion = Criterion.Where(F => !F(((DestinationNode)partialResult.Node).Destination));

                if (RemainingCriterion.Count() == 0)
                {
                    FinalResults.Add(partialResult);
                }
                else
                {
                    var remainingResults = SearchLocToDest(partialResult.Node, partialResult.Node.Time, Direction, RemainingCriterion, partialResult.PathCost);

                    if (remainingResults.Count() == 0) continue;

                    foreach (var result in remainingResults)
                    {
                        FinalResults.Add(ConcatResults(partialResult, result.Copy(), Direction));
                    }
                }
            }

            // ensure that destination sets are unique
            var UniqueDestSet = new HashSet<IEnumerable<IDestination>>();
            var UniqueResults = FinalResults.Where(result => 
                {
                    var searchresult = new SearchResult(result);
                    return UniqueDestSet.Add(searchresult.Destinations);
                }); 

            return UniqueResults;
        }

        private IEnumerable<NodeInfo> SearchDestToLoc(IEnumerable<NodeInfo> NodeInfos, TimeDirection Direction, ILocation EndLocation)
        {
            var FinalResults = new List<NodeInfo>();

            foreach (var info in NodeInfos)
            {
                var StartNodeInfos = GetClosestGtfsNodeInfos(info.Node, info.Node.Time, Direction, info.PathCost);

                Func<INode, bool> GoalCheck = node =>
                    {
                        return node as LocationGoalNode != null;
                    };

                var results = Dijkstras(StartNodeInfos, GoalCheck, Direction, EndLocation);
                FinalResults.Add(ConcatResults(results.First(), info, Direction));
            }

            return FinalResults;
        }

        public IEnumerable<SearchResult> Search(ILocation startLocation, ILocation endLocation, 
            DateTime startTime, TimeDirection direction, IEnumerable<Func<IDestination, bool>> criteria)
        {
            var startToDestinations = SearchLocToDest(startLocation, startTime, direction, criteria, TimeSpan.FromSeconds(0));
            var finalResults = SearchDestToLoc(startToDestinations, direction, endLocation);

            return finalResults
                .Select(info => new SearchResult(info))
                .ToArray();
        }
    }
}
