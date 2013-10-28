using System;

namespace Model.Sorta
{
    public class StopTime
    {
        public int Id { get; set; }

        #region Navigation Properties

        public virtual Trip Trip { get; set; }
        public virtual Stop Stop { get; set; }

        #endregion

        #region CSV

        public int TripId { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public int StopId { get; set; }
        public int Sequence { get; set; }
        public string Headsign { get; set; }
        public int? PickupType { get; set; }
        public int? DropOffType { get; set; }
        public double? ShapeDistanceTraveled { get; set; }

        #endregion
    }
}