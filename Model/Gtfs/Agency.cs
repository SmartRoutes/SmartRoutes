using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Gtfs
{
    public class Agency
    {
        private ICollection<Route> _routes;

        public Agency()
        {
            _routes = new Collection<Route>();
        }

        #region Navigation Properties

        public virtual ICollection<Route> Routes
        {
            get { return _routes; }
            set { _routes = value; }
        }

        #endregion

        #region CSV

        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Timezone { get; set; }
        public string Language { get; set; }
        public string Phone { get; set; }
        public string FareUrl { get; set; }

        #endregion
    }
}