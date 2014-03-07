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
        internal FibHeapHandle<NodeBase, TimeSpan> handle;
        internal TimeSpan pathCost;
        internal NodeState state;
        public INode node { get; internal set; }
        public NodeInfo parent;

        public NodeInfo Copy()
        {
            var copy = new NodeInfo();
            copy.pathCost = pathCost;
            copy.node = node;
            if (parent != null) copy.parent = parent.Copy();
            return copy;
        }
    }
}