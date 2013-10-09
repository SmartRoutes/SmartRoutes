using System;

namespace Model.Sorta
{
    public class ServiceException
    {
        public int Id { get; set; }
        public virtual Service Service { get; set; }

        #region CSV

        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public int ServiceExemptionTypeId { get; set; }

        #endregion
    }
}