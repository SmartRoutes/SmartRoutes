using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    class ComparerForDestinations : IComparer<IDestinationNode>
    {
        public int Compare(IDestinationNode x, IDestinationNode y)
        {
            return (int)(x.Time - y.Time).TotalMilliseconds;
        }
    }
}
