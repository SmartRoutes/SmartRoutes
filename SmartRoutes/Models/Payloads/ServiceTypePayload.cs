using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models.Payloads
{
    /// <summary>
    /// Payload object representing a service type to use in the search.
    /// </summary>
    public class ServiceTypePayload
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ServiceTypePayload()
        {

        }

        /// <summary>
        /// Name of the service type.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Flag indicating if the service type is checked.
        /// </summary>
        public bool Checked
        {
            get;
            set;
        }
    }
}