using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class AccreditationModel
    {
        public AccreditationModel()
        {

        }

        public AccreditationModel(string name, string description, Uri url)
        {
            this.Name = name;
            this.Description = description;
            this.URL = url;
            this.Checked = false;
        }

        /// <summary>
        /// The name of the accreditation agency to display to the user.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// A brief description of the accreditation agency to display to the user.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// A url that takes the user to a website that provides additional information.
        /// </summary>
        public Uri URL
        {
            get;
            set;
        }

        public bool Checked
        {
            get;
            set;
        }
    }
}