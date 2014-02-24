using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Heap;
using SmartRoutes.Graph.Node;

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

        public NodeInfo Copy()
        {
            var copy = new NodeInfo();
            copy.pathCost = pathCost;
            copy.node = node;
            if (parent != null) copy.parent = parent.Copy();
            return copy;
        }
    }

    public static class ExtensionMethods
    {



    }
}
