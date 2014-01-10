using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Graph.Comparers
{
    class ComparerForStopSorting : IComparer<Stop>
    {
        public int Compare(Stop stop1, Stop stop2)
        {
            return stop1.Id - stop2.Id;
        }
    }

    class ComparerForStopTimeSorting : IComparer<StopTime>
    {
        public int Compare(StopTime stoptime1, StopTime stoptime2)
        {
            if (stoptime1.Trip == null || stoptime2.Trip == null 
                || !stoptime1.Trip.ShapeId.HasValue || !stoptime2.Trip.ShapeId.HasValue)
            {
                throw new ArgumentNullException("StopTime.Trip.ShapeID has not been populated.");
            }
            else if (stoptime1.Trip.ShapeId.Value == stoptime2.Trip.ShapeId.Value)
            {
                return stoptime1.StopId - stoptime2.StopId;
            }
            else
            {
                return stoptime1.Trip.ShapeId.Value - stoptime2.Trip.ShapeId.Value;
            }
        }
    }
}
