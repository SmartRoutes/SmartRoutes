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
        internal TimeSpan travelTime;
        internal NodeState state;
        public INode node;
        public NodeInfo parent;
    }

    public static class ExtensionMethods
    {
        public static List<NodeInfo> Dijkstras(IEnumerable<INode> StartNodes, Func<INode, bool> GoalCheck, TimeDirection direction)
        {
            var Results = new List<NodeInfo>();
            var SearchInfo = new Dictionary<NodeBase, NodeInfo>();
            var heap = new FibonacciHeap<NodeBase, TimeSpan>();

            // assign search info to StartNodes and place them in queue
            foreach (var node in StartNodes)
            {
                var nodeInfo = new NodeInfo();
                nodeInfo.node = node;
                nodeInfo.state = NodeState.Closed;
                nodeInfo.travelTime = new TimeSpan(0);
                nodeInfo.handle = heap.Insert(node.BaseNode, nodeInfo.travelTime);
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
                        neighborInfo.travelTime = (direction == TimeDirection.Forwards)
                            ? currentInfo.travelTime + (neighbor.Time - current.Time)
                            : currentInfo.travelTime + (current.Time - neighbor.Time);
                        neighborInfo.handle = heap.Insert(neighbor.BaseNode, neighborInfo.travelTime);
                        SearchInfo.Add(neighbor.BaseNode, neighborInfo);
                    }
                    else
                    { 
                        // neighbor is in queue, check state
                        if (neighborInfo.state == NodeState.Open)
                        {
                            // update neighborInfo if this route is better
                            TimeSpan newTravelTime = (direction == TimeDirection.Forwards)
                                ? currentInfo.travelTime + (neighbor.Time - current.Time)
                                : currentInfo.travelTime + (current.Time - neighbor.Time);
                            if (newTravelTime < neighborInfo.travelTime)
                            {
                                // update search info and update queue for new key
                                //neighborInfo.node = current;
                                neighborInfo.travelTime = newTravelTime;
                                neighborInfo.parent = currentInfo;
                                heap.UpdateKey(neighborInfo.handle, newTravelTime);
                            }
                        }
                    }
                }

                // and we're done with current
                currentInfo.state = NodeState.Closed;
            }

            return Results;
        }
    }
}
