using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    class ComparerForChildCares : IComparer<IChildcareNode>
    {
        public int Compare(IChildcareNode x, IChildcareNode y)
        {
            return (int)(x.Time - y.Time).TotalMilliseconds;
        }
    }
}
