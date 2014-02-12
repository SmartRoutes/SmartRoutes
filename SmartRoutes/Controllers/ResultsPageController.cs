﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Controllers
{
    public class ResultsPageController : Controller
    {
        //
        // GET: /ResultsPage/

        public ActionResult ResultViewHtml()
        {
            return PartialView("~/Views/Results/_ResultView.cshtml");
        }
    }
}
