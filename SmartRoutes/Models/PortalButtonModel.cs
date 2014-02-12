using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// The model for a portal button.
    /// </summary>
    public class PortalButtonModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">The text for the button.</param>
        /// <param name="path">The link path for the button.</param>
        /// <param name="id">The ID for the button.</param>
        public PortalButtonModel(string text, string path, string id)
        {
            this.Text = text;
            this.Path = path;
            this.ID = id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Same as above.</param>
        /// <param name="path">Same as above.</param>
        public PortalButtonModel(string text, string path)
        {
            this.Text = text;
            this.Path = path;
            this.ID = string.Empty;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PortalButtonModel()
        {
        }

        /// <summary>
        /// The text for the button.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// The link for the button.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// The ID for the button.
        /// </summary>
        public string ID
        {
            get;
            set;
        }
    }
}