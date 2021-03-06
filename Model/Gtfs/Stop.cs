﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Gtfs
{
    public class Stop : ILocation
    {
        private ICollection<Stop> _childStops;
        private ICollection<StopTime> _stopTimes;

        public Stop()
        {
            _childStops = new Collection<Stop>();
            _stopTimes = new Collection<StopTime>();
        }

        #region Navigation Properties

        public virtual Stop ParentStop { get; set; }

        public virtual ICollection<Stop> ChildStops
        {
            get { return _childStops; }
            set { _childStops = value; }
        }

        public virtual ICollection<StopTime> StopTimes
        {
            get { return _stopTimes; }
            set { _stopTimes = value; }
        }

        #endregion

        #region CSV

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? ZoneId { get; set; }
        public string Url { get; set; }
        public int? TypeId { get; set; }
        public int? ParentId { get; set; }
        public string Timezone { get; set; }
        public int? WheelchairBoarding { get; set; }

        #endregion
    }
}