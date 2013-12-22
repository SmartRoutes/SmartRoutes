using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    public class ComparerForTransferSorting : IComparer<IGtfsNode>
    {
        // sorts nodes first by StopID, second by by Time
        // allows for transfer connections to be made by iteration
        public int Compare(IGtfsNode node1, IGtfsNode node2)
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