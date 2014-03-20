using System;
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
        /// <summary>
        /// The nodes in chronological order, with all intermediate nodes.
        /// Example: AAAAA BBBBBBB 1 CCCCCC 2 DDDDD
        ///   (A, B, C, D = GTFS stop times)
        ///   (1, 2 = destinations)
        /// </summary>
        public IEnumerable<NodeInfo> LongResults { get; private set; }

        /// <summary>
        /// The nodes in chronological order, without intermediate nodes.
        /// Example: AA BB 1 CC 2 DD
        ///   (A, B, C, D = GTFS stop times)
        ///   (1, 2 = destinations)
        /// </summary>
        public IEnumerable<NodeInfo> ShortResults { get; private set; }

        public IEnumerable<IDestination> Destinations { get; private set; }

        private TimeDirection OriginalSearchDirection;

        public SearchResult(NodeInfo info)
        {
            IList<NodeInfo> longResults = new List<NodeInfo>();
            IList<NodeInfo> shortResults = new List<NodeInfo>();

            var currentChild = info;
            while (currentChild != null)
            {
                longResults.Add(currentChild);
                currentChild = currentChild.Parent;
            }

            // make sure results are in ascending time
            if (info.Parent.Node.Time < info.Node.Time)
            {
                longResults = longResults.Reverse().ToList();
                OriginalSearchDirection = TimeDirection.Backwards;
            }
            else
            {
                OriginalSearchDirection = TimeDirection.Forwards;
            }

            // create short results, which only include bus route end-points and destinations
            shortResults.Add(longResults.First());
            NodeInfo previous = null;
            IGtfsNode previousGtfs = null;
            foreach (NodeInfo current in longResults)
            {
                if (previous == null)
                {
                    previous = current;
                    previousGtfs = previous.Node as IGtfsNode;
                    continue;
                }
                var currentGtfs = current.Node as IGtfsNode;
                if (previousGtfs != null && (currentGtfs == null || previousGtfs.TripId != currentGtfs.TripId))
                {
                    shortResults.Add(previous);
                }
                if (previousGtfs == null || currentGtfs == null || previousGtfs.TripId != currentGtfs.TripId)
                {
                    shortResults.Add(current);
                }
                previous = current;
                previousGtfs = previous.Node as IGtfsNode;
            }
            if (previousGtfs != null)
            {
                shortResults.Add(previous);
            }

            // set the container properties
            LongResults = longResults;
            ShortResults = shortResults;
            Destinations = ShortResults
                .Select(ni => ni.Node)
                .OfType<IDestinationNode>()
                .Select(dn => dn.Destination)
                .ToList();
        }

        public SearchResult Concat(SearchResult newResult)
        {
            if (OriginalSearchDirection != newResult.OriginalSearchDirection)
            {
                throw new Exception("Cannot concat result, search directions not compatible.");
            }

            if (OriginalSearchDirection == TimeDirection.Forwards)
            {
                if (LongResults.Last().Node.Time < newResult.LongResults.First().Node.Time)
                {
                    LongResults.Last().Parent = newResult.LongResults.First();
                    return new SearchResult(LongResults.First());
                }
                else if (LongResults.First().Node.Time >= newResult.LongResults.Last().Node.Time)
                {
                    newResult.LongResults.Last().Parent = LongResults.First();
                    return new SearchResult(newResult.LongResults.First());
                }
                else
                {
                    throw new Exception("Cannot concat result, search times overlap.");
                }
            }
            else
            {
                if (LongResults.First().Node.Time > newResult.LongResults.Last().Node.Time)
                {
                    LongResults.First().Parent = newResult.LongResults.Last();
                    return new SearchResult(LongResults.Last());
                }
                else if (newResult.LongResults.First().Node.Time >= LongResults.Last().Node.Time)
                {
                    newResult.LongResults.First().Parent = LongResults.Last();
                    return new SearchResult(newResult.LongResults.Last());
                }
                else
                {
                    throw new Exception("Cannot concat result, search times overlap.");
                }
            }
        }
    }
}