using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Sorta
{
    public class Route
    {
        private ICollection<Trip> _trips;

        public Route()
        {
            _trips = new Collection<Trip>();
        }

        #region Navigation Properties

        public virtual Agency Agency { get; set; }

        public virtual ICollection<Trip> Trips
        {
            get { return _trips; }
            set { _trips = value; }
        }

        #endregion

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