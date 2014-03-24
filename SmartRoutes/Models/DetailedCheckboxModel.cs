using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model for a detailed checkbox view.
    /// </summary>
    public class DetailedCheckboxModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DetailedCheckboxModel()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name to use for the detailed checkbox (actually should
        /// be the text property).</param>
        /// <param name="description">Description for the item in the checkbox.</param>
        public DetailedCheckboxModel(string name, string description)
        {
            this.Name = name;
            this.Description = description;
            this.Checked = true;
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
        /// Flag indicating if the box is checked.
        /// </summary>
        public bool Checked
        {
            get;
            set;
        }
    }
}