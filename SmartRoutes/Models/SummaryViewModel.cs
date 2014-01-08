using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Models
{
    public class SummaryViewModel
    {
        public SummaryViewModel(string title, string text)
        {
            this.Title = title;
            this.Text = text;
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
    }
}
