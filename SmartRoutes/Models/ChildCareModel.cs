using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model describing a child care.
    /// </summary>
    public class ChildCareModel
    {
        public ChildCareModel()
        {

        }

        /// <summary>
        /// The name of a child care.
        /// </summary>
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

        /// <summary>
        /// A link to some review page for a childcare.
        /// </summary>
        public string ReviewLink
        {
            get;
            set;
        }

        /// <summary>
        /// The address of the child care (if available).
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// The phone number for the child care (if available).
        /// </summary>
        public string PhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Collection representing the business hours for the child care.
        /// </summary>
        public IEnumerable<BusinessHoursModel> Hours
        {
            get;
            set;
        }
    }
}