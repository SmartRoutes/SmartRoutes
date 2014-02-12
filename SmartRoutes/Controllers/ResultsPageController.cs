using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Controllers
{
    /// <summary>
    /// Controller that handles requests for the results page.
    /// </summary>
    public class ResultsPageController : Controller
    {
        //
        // GET: /ResultsPage/

        /// <summary>
        /// Gets the raw HTML for the result view.
        /// </summary>
        /// <returns>The HTML for the result view.</returns>
        public ActionResult ResultViewHtml()
        {
            return PartialView("~/Views/Results/_ResultView.cshtml");
        }
    }
}
