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

    public enum Direction 
    { 
        Upwind, Downwind 
    }

    public class NodeInfo
    {
        internal FibHeapHandle<INode, TimeSpan> handle;
        internal TimeSpan travelTime;
        internal NodeState state;
        public INode node;
        public NodeInfo parent;
    }

    public static class ExtensionMethods
    {
        public static List<NodeInfo> Dijkstras(IEnumerable<INode> StartNodes, Func<INode, bool> GoalCheck, Direction direction)
        {
            var Results = new List<NodeInfo>();
            var SearchInfo = new Dictionary<INode, NodeInfo>();
            var heap = new FibonacciHeap<INode, TimeSpan>();

            // assign search info to StartNodes and place them in queue
            foreach (var node in StartNodes)
            {
                var nodeInfo = new NodeInfo();
                nodeInfo.node = node;
                nodeInfo.state = NodeState.Closed;
                nodeInfo.travelTime = new TimeSpan(0);
                nodeInfo.handle = heap.Insert(node, nodeInfo.travelTime);
                SearchInfo.Add(node, nodeInfo);
            }

            while (!heap.Empty())
            {
                INode current = heap.DeleteMin();

                // get search info
                NodeInfo currentInfo = null;
                if (!SearchInfo.TryGetValue(current, out currentInfo))
                { 
                    throw new KeyNotFoundException("Node removed from heap did not have associated search info: ");
                }

                // check for completion
                if (GoalCheck(current))
                {
                    // ensure node at this location has not already been added to Results
                    bool uniqueLocation = true;

                    foreach (var result in Results)
                    {
                        if (current.Longitude == result.node.Longitude && current.Latitude == result.node.Latitude)
                        {
                            uniqueLocation = false;
                        }
                    }

                    if (uniqueLocation)
                    {
                        Results.Add(currentInfo);
                    }

                    continue; // no need to continue searching along this path
                }

                // loop through neighbors and handle business
                var Neighbors = (direction == Direction.Upwind) ?
                    current.UpwindNeighbors : current.DownwindNeighbors;

                foreach (var neighbor in Neighbors)
                {
                    NodeInfo neighborInfo = null;
                    if (!SearchInfo.TryGetValue(neighbor, out neighborInfo))
                    { 
                        // node is new, give it search info and place in queue
                        neighborInfo = new NodeInfo();
                        neighborInfo.node = neighbor;
                        neighborInfo.parent = currentInfo;
                        neighborInfo.state = NodeState.Open;
                        neighborInfo.travelTime = (direction == Direction.Upwind)
                            ? currentInfo.travelTime + (neighbor.Time - current.Time)
                            : currentInfo.travelTime + (current.Time - neighbor.Time);
                        neighborInfo.handle = heap.Insert(neighbor, neighborInfo.travelTime);
                        SearchInfo.Add(neighbor, neighborInfo);
                    }
                    else
                    { 
                        // neighbor is in queue, check state
                        if (neighborInfo.state == NodeState.Open)
                        {
                            // update neighborInfo if this route is better
                            TimeSpan newTravelTime = (direction == Direction.Upwind)
                                ? currentInfo.travelTime + (neighbor.Time - current.Time)
                                : currentInfo.travelTime + (current.Time - neighbor.Time);
                            if (newTravelTime < neighborInfo.travelTime)
                            {
                                // update search info and update queue for new key
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
