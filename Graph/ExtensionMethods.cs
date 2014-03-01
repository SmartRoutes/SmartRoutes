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

    // class which extracts info from NodeInfo and removes references to graph
    // from Search's return value
    public class SearchResult
    {
        public List<NodeInfo> LongResults { get; private set; }
        public List<NodeInfo> ShortResults { get; private set; }

        public SearchResult(NodeInfo info)
        {
            LongResults = new List<NodeInfo>();
            ShortResults = new List<NodeInfo>();

            var current = info;

            while (current != null)
            {
                LongResults.Add(current);
                current = current.parent;
            }

            // make sure results are in ascending time
            if (info.parent.node.Time < info.node.Time)
            {
                LongResults.Reverse();
            }

            // create short results, which only include bus route end-points and destinations
            ShortResults.Add(LongResults.First());

            for (int i = 1; i < LongResults.Count - 1; i++)
            {
                var prev = LongResults[i - 1];
                var curr = LongResults[i];
                var next = LongResults[i + 1];

                if (prev.node is IDestinationNode || curr.node is IDestinationNode || next.node is IDestinationNode)
                {
                    ShortResults.Add(curr);
                }
                else
                {
                    var prevGtfs = prev.node as IGtfsNode;
                    var currGtfs = curr.node as IGtfsNode;
                    var nextGtfs = curr.node as IGtfsNode;

                    if (prevGtfs != null && currGtfs != null)
                    {
                        if (prevGtfs.BlockId != currGtfs.BlockId)
                        {
                            ShortResults.Add(curr);
                        }
                    }
                    else if (currGtfs != null && nextGtfs != null)
                    {
                        if (currGtfs.BlockId != nextGtfs.BlockId)
                        {
                            ShortResults.Add(curr);
                        }
                    }
                }
            }

            ShortResults.Add(LongResults.Last());
        }
    }

    public static class ExtensionMethods
    {



    }
}
