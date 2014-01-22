using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class AccreditationModel : DetailedCheckboxModel
    {
        public AccreditationModel()
        {

        }

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