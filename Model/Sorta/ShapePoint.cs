namespace Model.Sorta
{
    public class ShapePoint
    {
        public int Id { get; set; }
        public virtual Shape Shape { get; set; }

        #region CSV

        public int ShapeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Sequence { get; set; }
        public double? DistanceTraveled { get; set; }

        #endregion
    }
}