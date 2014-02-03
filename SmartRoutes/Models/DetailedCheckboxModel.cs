using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class DetailedCheckboxModel
    {
        public DetailedCheckboxModel()
        {

        }

        public DetailedCheckboxModel(string name, string description)
        {
            this.Name = name;
            this.Description = description;
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

        public bool Checked
        {
            get;
            set;
        }
    }
}