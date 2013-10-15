using System;

namespace Model.Odjfs
{
    public abstract class ChildCare
    {
        public int Id { get; set; }

        #region HTML

        public string Type { get; set; }
        public string UrlNumber { get; set; }
        public string PageNumber { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
        public string County { get; set; }
        public string PhoneNumber { get; set; }

        #endregion
    }
}