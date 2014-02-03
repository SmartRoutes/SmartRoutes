using System.Collections.Generic;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    public class ComparerForTripSorting : IComparer<IGtfsNode>
    {
        // sorts nodes first by TripId, second by Sequence
        // allows for trip connections to be made by iteration
        public int Compare(IGtfsNode node1, IGtfsNode node2)
        {
            if (node1.TripId == node2.TripId)
            {
                return node1.Sequence - node2.Sequence;
            }
            return node1.TripId - node2.TripId;
        }
    }
}