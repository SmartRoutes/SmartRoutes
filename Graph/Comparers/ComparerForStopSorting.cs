using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.Graph.Comparers
{
    class ComparerForStopSorting : IComparer<Stop>
    {
        public int Compare(Stop stop1, Stop stop2)
        {
            return stop1.Id - stop2.Id;
        }
    }
}
