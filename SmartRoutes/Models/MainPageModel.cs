using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Models
{
    /// <summary>
    /// The model used for the main page.
    /// </summary>
    public class MainPageModel : Controller
    {
        /// <summary>
        /// The title for the description view.
        /// </summary>
        public string DescriptionTitle
        {
            get
            {
                return "Description";
            }
        }
    }
}
