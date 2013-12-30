using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    public class ComparerForTripSorting : IComparer<IGtfsNode>
    {
        // sorts nodes first by TripID, second by Sequence
        // allows for trip connections to be made by iteration
        public int Compare(IGtfsNode node1, IGtfsNode node2)
        {
            if (node1.TripID == node2.TripID)
            {
                return node1.Sequence - node2.Sequence;
            }
            else
            {
                return node1.TripID - node2.TripID;
            }
        }
    }
}
