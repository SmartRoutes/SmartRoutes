namespace Model.Sorta
{
    public class Stop
    {
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
    }
}