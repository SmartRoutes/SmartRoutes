namespace Model.Sorta
{
    public class Route
    {
        public virtual Agency Agency { get; set; }

        #region CSV

        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string AgencyId { get; set; }

        public string LongName { get; set; }
        public int RouteTypeId { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }

        #endregion
    }
}