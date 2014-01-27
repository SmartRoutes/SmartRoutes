using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;
using SmartRoutes.Heap;

namespace SmartRoutes.Graph
{
    internal enum NodeState
    {
        Open, Closed
    }

    public enum TimeDirection 
    { 
        Backwards, Forwards 
    }

    public class NodeInfo
    {
        internal FibHeapHandle<NodeBase, TimeSpan> handle;
        internal TimeSpan pathCost;
        internal NodeState state;
        public INode node;
        public NodeInfo parent;
    }

    public static class ExtensionMethods
    {
        public static List<NodeInfo> Dijkstras(IEnumerable<INode> StartNodes, Func<INode, bool> GoalCheck, TimeDirection direction)
        {
            var Results = new List<NodeInfo>();

            if (StartNodes.Count() == 0) return Results;

            var SearchInfo = new Dictionary<NodeBase, NodeInfo>();
            var heap = new FibonacciHeap<NodeBase, TimeSpan>();

            // find base time
            DateTime BaseTime = StartNodes.First().Time;
            foreach (var node in StartNodes)
            {
                if (direction == TimeDirection.Backwards)
                {
                    if (node.Time > BaseTime) BaseTime = node.Time;
                }
                else
                {
                    if (node.Time < BaseTime) BaseTime = node.Time;
                }
            }

            // assign search info to StartNodes and place them in queue
            foreach (var node in StartNodes)
            {
                var nodeInfo = new NodeInfo();
                nodeInfo.node = node;
                nodeInfo.state = NodeState.Closed;
                nodeInfo.pathCost = (direction == TimeDirection.Backwards)
                    ? BaseTime - node.Time
                    : node.Time - BaseTime;
                nodeInfo.handle = heap.Insert(node.BaseNode, nodeInfo.pathCost);
                SearchInfo.Add(node.BaseNode, nodeInfo);
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

                var current = currentInfo.node;

                //Console.WriteLine("{0} --------- {1}", current.BaseNode.Name, current.Time);
                //System.Threading.Thread.Sleep(50);

                // check for completion
                if (GoalCheck(current))
                {
                    Results.Add(currentInfo);
                    currentInfo.state = NodeState.Closed;
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
                        neighborInfo.node = neighbor;
                        neighborInfo.parent = currentInfo;
                        neighborInfo.state = NodeState.Open;
                        neighborInfo.pathCost = GetNextPathCost(currentInfo, neighbor, direction);
                        neighborInfo.handle = heap.Insert(neighbor.BaseNode, neighborInfo.pathCost);
                        SearchInfo.Add(neighbor.BaseNode, neighborInfo);
                    }
                    else
                    { 
                        // neighbor is in queue, check state
                        if (neighborInfo.state == NodeState.Open)
                        {
                            // update neighborInfo if this route is better
                            TimeSpan newPathCost = GetNextPathCost(currentInfo, neighbor, direction);
                            if (newPathCost < neighborInfo.pathCost)
                            {
                                // update search info and update queue for new key
                                neighborInfo.pathCost = newPathCost;
                                neighborInfo.parent = currentInfo;
                                heap.UpdateKey(neighborInfo.handle, newPathCost);
                            }
                        }
                    }
                }

                // and we're done with current
                currentInfo.state = NodeState.Closed;
            }

            return Results;
        }

        private static TimeSpan GetNextPathCost(NodeInfo currentInfo, INode neighbor, TimeDirection direction)
        {
            TimeSpan travelTimeBetween = neighbor.Time - currentInfo.node.Time;
            TimeSpan pathCost = (direction == TimeDirection.Forwards)
                ? currentInfo.pathCost + travelTimeBetween
                : currentInfo.pathCost - travelTimeBetween;

            // penalize transfers
            var gtfsCurrent = currentInfo.node as IGtfsNode;
            var gtfsNeighbor = neighbor as IGtfsNode;
            if (gtfsCurrent != null && gtfsNeighbor != null)
            {
                if (gtfsCurrent.TripId != gtfsNeighbor.TripId)
                {
                    pathCost += TimeSpan.FromMinutes(20);
                }
            }

            return pathCost;
        }
    }
}
