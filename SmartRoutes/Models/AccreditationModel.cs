using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model representing an accreditation to sent to the client.
    /// </summary>
    public class AccreditationModel : DetailedCheckboxModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AccreditationModel()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the accreditation.</param>
        /// <param name="description">Description for the accreditation.</param>
        /// <param name="url">URL for the accreditation.</param>
        public AccreditationModel(string name, string description, Uri url)
            : base(name, description)
        {
            this.URL = url;
        }

        /// <summary>
        /// A url that takes the user to a website that provides additional information.
        /// </summary>
        public Uri URL
        {
            get;
            set;
        }
    }
}