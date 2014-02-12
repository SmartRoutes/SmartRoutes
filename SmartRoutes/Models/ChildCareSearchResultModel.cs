using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class ChildCareSearchResultModel
    {
        public ChildCareSearchResultModel()
        {

        }

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