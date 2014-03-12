using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model describing the results of a child care search.
    /// </summary>
    public class ChildCareSearchResultsModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChildCareSearchResultsModel()
        {
        }

        /// <summary>
        /// Collection of child care information.
        /// </summary>
        public IEnumerable<ChildCareModel> ChildCares
        {
            get
            {
                return this.childCares.AsEnumerable();
            }
        }

        /// <summary>
        /// Collection of child care routes.
        /// </summary>
        public IEnumerable<ChildCareRouteModel> Routes
        {
            get
            {
                return this.routes.AsEnumerable();
            }
        }

        /// <summary>
        /// Adds a child care route to the end of the collection.
        /// </summary>
        /// <param name="route">The route to add.</param>
        public void AddChildCareRoute(ChildCareRouteModel route)
        {
            this.routes.Add(route);
        }

        /// <summary>
        /// Adds a child care model to the end of the collection.
        /// </summary>
        /// <param name="childCare">The child care model to add.</param>
        public void AddChildCare(ChildCareModel childCare)
        {
            this.childCares.Add(childCare);
        }

        private IList<ChildCareRouteModel> routes = new List<ChildCareRouteModel>();
        private IList<ChildCareModel> childCares = new List<ChildCareModel>();
    }
}