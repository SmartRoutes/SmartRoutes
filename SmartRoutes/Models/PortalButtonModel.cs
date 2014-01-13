using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartRoutes.Models
{
    public class PortalButtonModel
    {
        public PortalButtonModel(string text, string path, string id)
        {
            this.Text = text;
            this.Path = path;
            this.ID = id;
        }

        public PortalButtonModel()
        {
        }

        public string Text
        {
            get;
            set;
        }

        public string Path
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