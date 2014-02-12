using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model describing the results of a child care search.
    /// </summary>
    public class ChildCareSearchResultModel
    {
        public ChildCareSearchResultModel()
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

        /// <summary>
        /// This should be the summary of route connections
        /// to be displayed with a route identifier (generally a number)
        /// as each element.  i.e. 2 -> 4 -> 1
        /// </summary>
        public IEnumerable<string> RouteSummary
        {
            get;
            set;
        }
    }
}