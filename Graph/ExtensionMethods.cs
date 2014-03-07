using System;
using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Graph.Heap;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;

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
        public IEnumerable<NodeInfo> LongResults { get; private set; }
        public IEnumerable<NodeInfo> ShortResults { get; private set; }
        public IEnumerable<IDestination> Destinations { get; private set; } 

        public SearchResult(NodeInfo info)
        {
            IList<NodeInfo> longResults = new List<NodeInfo>();
            IList<NodeInfo> shortResults = new List<NodeInfo>();

            var current = info;

            while (current != null)
            {
                longResults.Add(current);
                current = current.parent;
            }

            // make sure results are in ascending time
            if (info.parent.node.Time < info.node.Time)
            {
                longResults = longResults.Reverse().ToList();
            }

            // create short results, which only include bus route end-points and destinations
            shortResults.Add(longResults.First());

            for (int i = 1; i < longResults.Count - 1; i++)
            {
                var prev = longResults[i - 1];
                var curr = longResults[i];
                var next = longResults[i + 1];

                if (prev.node is IDestinationNode || curr.node is IDestinationNode || next.node is IDestinationNode)
                {
                    shortResults.Add(curr);
                }
                else
                {
                    var prevGtfs = prev.node as IGtfsNode;
                    var currGtfs = curr.node as IGtfsNode;
                    var nextGtfs = next.node as IGtfsNode;

                    if (prevGtfs != null && currGtfs != null)
                    {
                        if (prevGtfs.BlockId != currGtfs.BlockId)
                        {
                            shortResults.Add(curr);
                        }
                    }
                    else if (currGtfs != null && nextGtfs != null)
                    {
                        if (currGtfs.BlockId != nextGtfs.BlockId)
                        {
                            shortResults.Add(curr);
                        }
                    }
                }
            }

            shortResults.Add(longResults.Last());

            // set the container properties
            LongResults = longResults;
            ShortResults = shortResults;
            Destinations = ShortResults
                .Select(ni => ni.node)
                .OfType<IDestinationNode>()
                .Select(dn => dn.Destination)
                .ToList();
        }
    }
}
