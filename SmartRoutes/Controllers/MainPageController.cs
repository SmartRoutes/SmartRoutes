using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartRoutes.Models;

namespace SmartRoutes.Controllers
{
    /// <summary>
    /// This controller handles requests from the main page.
    /// </summary>
    public class MainPageController : Controller
    {
        /// <summary>
        /// Gets the content view.
        /// </summary>
        /// <returns>The content view HTML.</returns>
        public ActionResult Index()
        {
            return View("ContentView");
        }

        /// <summary>
        /// Gets the website description for the main page.
        /// </summary>
        /// <returns>A view that is prepopulated with information about the website.</returns>
        public ActionResult DescriptionView()
        {
            SummaryViewModel summaryModel = new SummaryViewModel(Resources.descriptionTitleText,
                                                                 Resources.descriptionText,
                                                                 "sr-description-view");
            return PartialView("_SummaryView", summaryModel);
        }

        /// <summary>
        /// Gets a sample map to display on the main page.
        /// </summary>
        /// <returns>A sample map images to display on the page.</returns>
        public ActionResult SampleMapView()
        {
            ImageViewModel sampleMapModel = new ImageViewModel("Content/Images/sample_map.png",
                                                               "Sample map",
                                                               "Sample Map",
                                                               "sr-sample-map");
            return PartialView("_ImageView", sampleMapModel);
        }

        /// <summary>
        /// Gets the portal view to display on the left-hand side of the main page.
        /// </summary>
        /// <returns>The HTML for the portal view.</returns>
        public ActionResult PortalView()
        {
            PortalViewModel portalViewModel = new PortalViewModel("sr-portal-view-main-page");
            portalViewModel.AddButtonModel(new PortalButtonModel(Resources.portalTextPlan, "#/search", "sr-portal-button-plan"));
            portalViewModel.AddButtonModel(new PortalButtonModel(Resources.portalTextFeedback, "#/feedback", "sr-portal-button-feedback"));

            return PartialView("_PortalView", portalViewModel);
        }
    }
}
