using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartRoutes.Controllers
{
    public class ItineraryPageController : Controller
    {
        //
        // GET: /ItineraryPage/

        public ActionResult FetchChildCareServiceDescriptionViewHTML()
        {
            return PartialView("~/Views/Itinerary/_ChildCareServiceDescriptionView.cshtml");
        }
    }
}
