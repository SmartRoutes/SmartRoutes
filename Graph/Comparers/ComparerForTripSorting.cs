using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph.Node;

namespace Graph.Comparers
{
    class ComparerForTripSorting : IComparer<IMetroNode>
    {
        // sorts nodes first by TripID, second by Sequence
        // allows for trip connections to be made by iteration
        public int Compare(IMetroNode node1, IMetroNode node2)
        {
            if (node1.TripID() == node2.TripID())
            {
                return node1.Sequence() - node2.Sequence();
            }
            else
            {
                return node1.TripID() - node2.TripID();
            }
        }
    }
}
