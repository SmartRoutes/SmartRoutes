using System;

namespace SmartRoutes.Model.Gtfs
{
    public class ServiceException
    {
        public int Id { get; set; }

        #region Navigation Properties

        public virtual Service Service { get; set; }

        #endregion

        #region CSV

        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public int ServiceExemptionTypeId { get; set; }

        #endregion
    }
}