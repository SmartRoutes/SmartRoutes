using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartRoutes.Models;

namespace SmartRoutes.Controllers
{
    public class MainPageController : Controller
    {
        public ActionResult Index()
        {
            return View("ContentView");
        }

        public JsonResult DescriptionViewData()
        {
            SummaryViewModel summaryModel = new SummaryViewModel();// new SummaryViewModel(Resources.descriptionTitleText, Resources.descriptionText);
            // There's a security thing here about sensitive data and json get requests.
            // This data isn't sensitive.
            return Json(summaryModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SampleMapView()
        {
            ImageViewModel sampleMapModel = new ImageViewModel("Content/Images/sample_map.png", "Sample map", "Sample Map");
            return PartialView("_imageView", sampleMapModel);
        }
    }
}
