using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Models
{
    public class ImageViewModel
    {
        public ImageViewModel(string imageSource, string altText, string titleText, string id)
        {
            this.ImageSource = imageSource;
            this.AltText = altText;
            this.TitleText = titleText;
            this.ID = id;
        }

        public string ImageSource
        {
            get;
            set;
        }

        public string AltText
        {
            get;
            set;
        }

        public string TitleText
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