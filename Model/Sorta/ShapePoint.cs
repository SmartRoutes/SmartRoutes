namespace Model.Sorta
{
    public class ShapePoint
    {
        public int Id { get; set; }

        #region Navigation Properties

        public virtual Shape Shape { get; set; }

        #endregion

        #region CSV

        public int ShapeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Sequence { get; set; }
        public double? DistanceTraveled { get; set; }

        #endregion
    }
}