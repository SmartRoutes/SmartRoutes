using System.Collections.Generic;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    internal class ComparerForDestinations : IComparer<IDestinationNode>
    {
        public int Compare(IDestinationNode x, IDestinationNode y)
        {
            return (int) (x.Time - y.Time).TotalMilliseconds;
        }
    }
}