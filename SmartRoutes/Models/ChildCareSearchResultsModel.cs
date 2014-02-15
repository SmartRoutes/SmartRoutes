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
        public ChildCareSearchResultsModel()
        {

        }

        /// <summary>
        /// Collection of child care information.
        /// </summary>
        public IEnumerable<ChildCareModel> ChildCares
        {
            get;
            set;
        }


        public IEnumerable<ChildCareRouteModel> Routes
        {
            get;
            set;
        }
    }
}