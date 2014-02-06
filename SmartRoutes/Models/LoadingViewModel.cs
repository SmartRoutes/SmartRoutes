using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class LoadingViewModel
    {
        public LoadingViewModel(string id)
        {
            this.ID = id;
            this.LoadingImageModel = new ImageViewModel("/Content/Images/loading.gif", "Loading", "Loading", string.Empty, "sr-loading-image");
        }

        public ImageViewModel LoadingImageModel
        {
            get;
            set;
        }

        public string ID
        {
            get;
            set;
        }
    }
}