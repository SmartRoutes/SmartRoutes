using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class ChildCareModel
    {
        public ChildCareModel()
        {

        }

        public string ChildCareName
        {
            get;
            set;
        }

        /// <summary>
        /// A link for the child care.  Can be left blank
        /// if nothing is available.
        /// </summary>
        public string Link
        {
            get;
            set;
        }

        public string ReviewLink
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public string PhoneNumber
        {
            get;
            set;
        }

        public IEnumerable<BusinessHours> Hours
        {
            get;
            set;
        }
    }
}