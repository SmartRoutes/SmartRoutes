using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model for a service type.
    /// </summary>
    public class ServiceTypeModel : DetailedCheckboxModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ServiceTypeModel()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the service type.</param>
        /// <param name="description">Description for the service type.</param>
        public ServiceTypeModel(string name, string description)
            : base(name, description)
        {
        }
    }
}