using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model for the portal view.
    /// </summary>
    public class PortalViewModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The ID for the view.</param>
        public PortalViewModel(string id)
        {
            this.ID = id;
        }

        /// <summary>
        /// The ID for the view.
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Ordered collection of buttons to display in the view.
        /// </summary>
        public IEnumerable<PortalButtonModel> ButtonModels
        {
            get
            {
                return this.buttonModels;
            }
        }

        /// <summary>
        /// Adds the model for a button to this model.
        /// </summary>
        /// <param name="buttonModel">The button model to add.</param>
        public void AddButtonModel(PortalButtonModel buttonModel)
        {
            this.buttonModels.Add(buttonModel);
        }

        /// <summary>
        /// Ordered collection of button models to use in the view.
        /// </summary>
        private List<PortalButtonModel> buttonModels = new List<PortalButtonModel>();
    }
}