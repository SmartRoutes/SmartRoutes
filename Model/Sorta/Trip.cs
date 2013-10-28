using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Sorta
{
    public class Trip
    {
        private ICollection<StopTime> _stopTimes;

        public Trip()
        {
            _stopTimes = new Collection<StopTime>();
        }

        #region Navigation Properties

        public virtual ICollection<StopTime> StopTimes
        {
            get { return _stopTimes; }
            set { _stopTimes = value; }
        }

        public virtual Route Route { get; set; }
        public virtual Service Service { get; set; }
        public virtual Block Block { get; set; }
        public virtual Shape Shape { get; set; }

        #endregion

        #region CSV

        public int Id { get; set; }
        public int RouteId { get; set; }
        public int ServiceId { get; set; }
        public string Headsign { get; set; }
        public string ShortName { get; set; }
        public int? DirectionId { get; set; }
        public int? BlockId { get; set; }
        public int? ShapeId { get; set; }
        public int? WheelchairAccessible { get; set; }

        #endregion
    }
}