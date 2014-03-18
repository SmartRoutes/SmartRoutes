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

        private IEnumerable<IGtfsNode> GetDestinationNeighbors(IDestinationNode node, TimeDirection direction)
        {
            var returnList = new List<IGtfsNode>();
            var uniqueNodeBases = new HashSet<NodeBase>();

            var q = new Queue<IDestinationNode>();
            q.Enqueue(node);

            while (q.Count() > 0)
            {
                var current = q.Dequeue();
                var neighbors = (direction == TimeDirection.Forwards)
                    ? current.TimeForwardNeighbors
                    : current.TimeBackwardNeighbors;

                foreach (var neighbor in neighbors.OfType<IGtfsNode>())
                {
                    if (uniqueNodeBases.Add(neighbor.BaseNode))
                    {
                        returnList.Add(neighbor);
                    }
                }

                foreach (var neighbor in neighbors.OfType<IDestinationNode>())
                {
                    q.Enqueue(neighbor);
                }
            }

            return returnList;
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

        private IEnumerable<SearchResult> Dijkstras(IEnumerable<NodeInfo> StartNodeInfos, IEnumerable<Func<INode, bool>> Criteria, int maxCount, TimeDirection direction, ILocation EndLocation = null)
        {
            var searchKeyMgr = new SearchKeyManager(Criteria);

            var Results = new List<SearchResult>();

            // ensure that destination sets are unique
            var UniqueDestCollSet = new HashSet<IEnumerable<IDestination>>(new DestinationCollectionComparer());

            if (StartNodeInfos.Count() == 0) return Results;

            var SearchInfo = new Dictionary<Tuple<NodeBase, string>, NodeInfo>();
            var heap = new FibonacciHeap<Tuple<NodeBase, string>, TimeSpan>();

            // assign search info to StartNodes and place them in queue
            foreach (var nodeInfo in StartNodeInfos)
            {
                nodeInfo.UnsatisfiedCriteria = searchKeyMgr.UnsatisfiedCriteria(nodeInfo.Node);
                var nodeKey = new Tuple<NodeBase, string>(nodeInfo.Node.BaseNode, nodeInfo.UnsatisfiedCriteria);
                nodeInfo.Handle = heap.Insert(nodeKey, nodeInfo.PathCost);
                SearchInfo.Add(nodeKey, nodeInfo);
            }

            while (!heap.Empty())
            {
                Tuple<NodeBase, string> currentSearchKey = heap.DeleteMin();

                // get search info
                NodeInfo currentInfo = null;
                if (!SearchInfo.TryGetValue(currentSearchKey, out currentInfo))
                {
                    throw new KeyNotFoundException("Node removed from heap did not have associated search info: ");
                }

                // stop searching if path cost is too large
                if (currentInfo.PathCost > TimeSpan.FromHours(2)) break;

                var current = currentInfo.Node;

                // check for completion
                if (currentInfo.UnsatisfiedCriteria == "")
                {
                    var currentResult = new SearchResult(currentInfo);

                    if (EndLocation != null)
                    {
                        Results.Add(currentResult);
                    }
                    else if (UniqueDestCollSet.Add(currentResult.Destinations))
                    {
                        Results.Add(currentResult);
                    }
                    
                    if (EndLocation != null || Results.Count() >= maxCount) break;
                    currentInfo.State = NodeState.Closed;
                    continue;
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
                    GoalInfo.UnsatisfiedCriteria = "";

                    var GoalSearchKey = new Tuple<NodeBase, string>(GoalInfo.Node.BaseNode, "");
                    GoalInfo.Handle = heap.Insert(GoalSearchKey, GoalInfo.PathCost);
                    SearchInfo.Add(GoalSearchKey, GoalInfo);
                }

                // determine neighbors. this is straightforward for gtfs nodes, less so for destination nodes
                IEnumerable<INode> Neighbors = null;

                if (current as IGtfsNode != null)
                {
                    Neighbors = (direction == TimeDirection.Forwards) ?
                        current.TimeForwardNeighbors : current.TimeBackwardNeighbors;
                }
                // if this is a destination which does not satisfy any new criteria,
                // there is no need to visit neighbors. otherwise search
                // cuts through destinations to avoid transfer pathcost penalty
                else if (currentInfo.UnsatisfiedCriteria != currentInfo.Parent.UnsatisfiedCriteria)
                {
                    Neighbors = GetDestinationNeighbors(current as IDestinationNode, direction);
                }

                // loop through neighbors and handle business
                foreach (var neighbor in Neighbors ?? new INode[0])
                {
                    if (neighbor.BaseNode == current.BaseNode) continue;

                    var neighborSearchKey = searchKeyMgr.neighborKey(neighbor, currentInfo);

                    NodeInfo neighborInfo = null;
                    if (!SearchInfo.TryGetValue(neighborSearchKey, out neighborInfo))
                    {
                        // neighbor is new, or neighbor has been reached but with different criteria satisfied
                        // give it search info and place in queue
                        neighborInfo = new NodeInfo();
                        neighborInfo.Node = neighbor;
                        neighborInfo.Parent = currentInfo;
                        neighborInfo.State = NodeState.Open;
                        neighborInfo.PathCost = GetNextPathCost(currentInfo, neighbor, direction);
                        neighborInfo.Handle = heap.Insert(neighborSearchKey, neighborInfo.PathCost);
                        neighborInfo.UnsatisfiedCriteria = neighborSearchKey.Item2;
                        SearchInfo.Add(neighborSearchKey, neighborInfo);
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

            return Results;
        }

        private IEnumerable<SearchResult> SearchLocToDest(ILocation StartLocation, DateTime StartTime, TimeDirection Direction, 
            IEnumerable<Func<IDestination, bool>> Criterion, int maxCount, TimeSpan BasePathCost)
        {
            var StartNodeInfos = GetClosestGtfsNodeInfos(StartLocation, StartTime, Direction, BasePathCost);
            var DijkstraCriteria = Criterion.Select(F =>
                {
                    return new Func<INode, bool>(n =>
                        {
                            var dest = n as IDestinationNode;

                            if (dest == null) return false;
                            else return F(dest.Destination);
                        });
                });

            var FinalResults = Dijkstras(StartNodeInfos, DijkstraCriteria, maxCount, Direction);

            return FinalResults;
        }

        private IEnumerable<SearchResult> SearchDestToLoc(IEnumerable<SearchResult> PartialResults, TimeDirection Direction, ILocation EndLocation, int maxCount)
        {
            var FinalResults = new List<SearchResult>();

            foreach (var result in PartialResults)
            {
                var info = (Direction == TimeDirection.Forwards)
                    ? result.LongResults.Last()
                    : result.LongResults.First();
                var StartNodeInfos = GetClosestGtfsNodeInfos(info.Node, info.Node.Time, Direction, info.PathCost);

                Func<INode, bool> GoalCheck = node =>
                    {
                        return node as LocationGoalNode != null;
                    };

                var results = Dijkstras(StartNodeInfos, new[] { GoalCheck }, maxCount, Direction, EndLocation);
                FinalResults.Add(result.Concat(results.First()));
            }

            return FinalResults;
        }

        public IEnumerable<SearchResult> Search(ILocation startLocation, ILocation endLocation,
            DateTime startTime, TimeDirection direction, IEnumerable<Func<IDestination, bool>> criteria, int maxCount)
        {
            var startToDestinations = SearchLocToDest(startLocation, startTime, direction, criteria, maxCount, TimeSpan.FromSeconds(0));
            var finalResults = SearchDestToLoc(startToDestinations, direction, endLocation, maxCount);

            return finalResults;
        }
    }
}
