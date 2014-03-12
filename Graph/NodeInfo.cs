using System;
using SmartRoutes.Graph.Heap;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph
{
    internal enum NodeState
    {
        Open, Closed
    }

    public class NodeInfo
    {
        internal FibHeapHandle<NodeBase, TimeSpan> Handle;
        internal NodeState State;
        public INode Node { get; internal set; }
        public NodeInfo Parent;
        public TimeSpan PathCost { get; internal set; }

        public NodeInfo Copy()
        {
            var copy = new NodeInfo();
            copy.PathCost = PathCost;
            copy.Node = Node;
            if (Parent != null) copy.Parent = Parent.Copy();
            return copy;
        }
    }
}