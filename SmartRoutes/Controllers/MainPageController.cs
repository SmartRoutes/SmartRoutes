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

        public ActionResult DescriptionView()
        {
            SummaryViewModel summaryModel = new SummaryViewModel(Resources.descriptionTitleText,
                                                                 Resources.descriptionText,
                                                                 "sr-description-view");
            return PartialView("_SummaryView", summaryModel);
        }

        public ActionResult SampleMapView()
        {
            ImageViewModel sampleMapModel = new ImageViewModel("Content/Images/sample_map.png",
                                                               "Sample map",
                                                               "Sample Map",
                                                               "sr-sample-map");
            return PartialView("_ImageView", sampleMapModel);
        }

        public ActionResult PortalView()
        {
            PortalViewModel portalViewModel = new PortalViewModel("sr-portal-view-main-page");
            portalViewModel.AddButtonModel(new PortalButtonModel(Resources.portalTextPlan, "", "sr-portal-button-plan"));
            portalViewModel.AddButtonModel(new PortalButtonModel(Resources.portalTextFeedback, "", "sr-portal-button-feedback"));

            return PartialView("_PortalView", portalViewModel);
        }
    }
}
