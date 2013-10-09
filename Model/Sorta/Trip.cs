namespace Model.Sorta
{
    public class Trip
    {
        public virtual Route Route { get; set; }
        public virtual Service Service { get; set; }
        public virtual Block Block { get; set; }
        public virtual Shape Shape { get; set; }

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