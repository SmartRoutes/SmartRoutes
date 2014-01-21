using System.Collections.Generic;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    public class ComparerForTransferSorting : IComparer<IGtfsNode>
    {
        // sorts nodes first by StopId, second by by Time
        // allows for transfer connections to be made by iteration
        public int Compare(IGtfsNode node1, IGtfsNode node2)
        {
            if (node1.StopId == node2.StopId)
            {
                return (int) (node1.Time - node2.Time).TotalSeconds;
            }
            return node1.StopId - node2.StopId;
        }
    }
}