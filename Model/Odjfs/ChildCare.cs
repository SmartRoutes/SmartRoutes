namespace Model.Odjfs
{
    public abstract class ChildCare
    {
        public int Id { get; set; }
        public string ChildCareType { get; set; }

        #region HTML

        public string ExternalUrlId { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
        public string County { get; set; }
        public string PhoneNumber { get; set; }

        #endregion
    }
}