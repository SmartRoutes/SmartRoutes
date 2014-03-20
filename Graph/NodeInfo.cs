using System;
using System.Collections.Generic;
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
        internal FibHeapHandle<Tuple<NodeBase, string>, TimeSpan> Handle;
        internal NodeState State;
        internal string UnsatisfiedCriteria;
        internal TimeSpan PathCost;
        public INode Node { get; internal set; }
        public NodeInfo Parent { get; internal set; }

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