using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    /// <summary>
    /// Payload object representing a child to use in the search.
    /// </summary>
    public class ChildInformationPayload
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChildInformationPayload()
        {

        }

        /// <summary>
        /// String indicating the age group of the child.
        /// </summary>
        public string AgeGroup
        {
            get;
            set;
        }
    }
}