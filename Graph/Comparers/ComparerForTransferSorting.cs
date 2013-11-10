using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph.Node;

namespace Graph.Comparers
{
    public class ComparerForTransferSorting : IComparer<IMetroNode>
    {
        // sorts nodes first by StopID, second by by Time
        // allows for transfer connections to be made by iteration
        public int Compare(IMetroNode node1, IMetroNode node2)
        {
            if (node1.StopID == node2.StopID)
            {
                return (int)(node1.Time - node2.Time).TotalSeconds;
            }
            else
            {
                return node1.StopID - node2.StopID;
            }
        }
    }
}