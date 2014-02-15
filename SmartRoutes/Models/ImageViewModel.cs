using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Models
{
    /// <summary>
    /// Model for an image.
    /// </summary>
    public class ImageViewModel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageSource">Path to the image.</param>
        /// <param name="altText">Alt text for the image.</param>
        /// <param name="titleText">Title for the image.</param>
        /// <param name="id">ID for the image in the page.</param>
        public ImageViewModel(string imageSource, string altText, string titleText, string id)
        {
            this.ImageSource = imageSource;
            this.AltText = altText;
            this.TitleText = titleText;
            this.ID = id;
            this.Class = string.Empty;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="imageSource">Same as above.</param>
        /// <param name="altText">Same as above.</param>
        /// <param name="titleText">Same as above.</param>
        /// <param name="id">Same as above.</param>
        /// <param name="className">Class to give the image.</param>
        public ImageViewModel(string imageSource, string altText, string titleText, string id, string className)
        {
            this.ImageSource = imageSource;
            this.AltText = altText;
            this.TitleText = titleText;
            this.ID = string.Empty;
            this.Class = className;
        }

        /// <summary>
        /// The server path to the image.
        /// </summary>
        public string ImageSource
        {
            get;
            set;
        }

        /// <summary>
        /// The alt text to display for the image.
        /// </summary>
        public string AltText
        {
            get;
            set;
        }

        /// <summary>
        /// The title text to display for the image.
        /// </summary>
        public string TitleText
        {
            get;
            set;
        }

        /// <summary>
        /// The ID to give to the image element.
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// The class to give to the image element.
        /// </summary>
        public string Class
        {
            get;
            set;
        }
    }
}