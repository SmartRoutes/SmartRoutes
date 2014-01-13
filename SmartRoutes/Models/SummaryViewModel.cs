using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Models
{
    public class SummaryViewModel
    {
        public SummaryViewModel(string title, string text, string id)
        {
            this.Title = title;
            this.Text = text;
            this.ID = id;
        }

        public SummaryViewModel()
        {
        }

        public string Title
        {
            get;
            set;
        }

        public string Text
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
