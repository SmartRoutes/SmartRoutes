using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph.Comparers
{
    public class ComparerForDisplay : IComparer<INode>
    {
        public int Compare(INode x, INode y)
        {
            return (int)(x.Time - y.Time).TotalMilliseconds;
        }
    }
}
