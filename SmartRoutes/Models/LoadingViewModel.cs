using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    /// <summary>
    /// The model for the loading animation.
    /// </summary>
    public class LoadingViewModel
    {
        public LoadingViewModel(string id)
        {
            this.ID = id;
            this.LoadingImageModel = new ImageViewModel("/Content/Images/loading.gif", "Loading", "Loading", string.Empty, "sr-loading-image");
        }

        /// <summary>
        /// The model for the image used in the loading animation.
        /// </summary>
        public ImageViewModel LoadingImageModel
        {
            get;
            set;
        }

        /// <summary>
        /// The ID for the loading animation element.
        /// </summary>
        public string ID
        {
            get;
            set;
        }
    }
}