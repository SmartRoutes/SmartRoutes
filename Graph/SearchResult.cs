using System.Collections.Generic;
using System.Linq;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
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
                current = current.Parent;
            }

            // make sure results are in ascending time
            if (info.Parent.Node.Time < info.Node.Time)
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

                if (prev.Node is IDestinationNode || curr.Node is IDestinationNode || next.Node is IDestinationNode)
                {
                    shortResults.Add(curr);
                }
                else
                {
                    var prevGtfs = prev.Node as IGtfsNode;
                    var currGtfs = curr.Node as IGtfsNode;
                    var nextGtfs = next.Node as IGtfsNode;

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
                .Select(ni => ni.Node)
                .OfType<IDestinationNode>()
                .Select(dn => dn.Destination)
                .ToList();
        }
    }
}