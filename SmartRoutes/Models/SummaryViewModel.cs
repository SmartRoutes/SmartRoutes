using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model for a summary text element.
    /// </summary>
    public class SummaryViewModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">Title for the summary area.</param>
        /// <param name="text">Text for the summary area.</param>
        /// <param name="id">ID for the summary area.</param>
        public SummaryViewModel(string title, string text, string id)
        {
            this.Title = title;
            this.Text = text;
            this.ID = id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SummaryViewModel()
        {
        }

        /// <summary>
        /// Title for the summary area.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Text for the summary area (the summary).
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// The ID for the summary element.
        /// </summary>
        public string ID
        {
            get;
            set;
        }
    }
}
