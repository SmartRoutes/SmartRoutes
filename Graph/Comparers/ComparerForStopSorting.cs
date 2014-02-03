using System.Collections.Generic;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph.Comparers
{
    internal class ComparerForStopSorting : IComparer<Stop>
    {
        public int Compare(Stop stop1, Stop stop2)
        {
            return stop1.Id - stop2.Id;
        }
    }
}