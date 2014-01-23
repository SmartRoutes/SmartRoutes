using System.Web;
using System.Web.Optimization;

namespace SmartRoutes
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/routing").Include(
                        "~/Scripts/sammy-{version}.js"));

            // Initialization order matters here.  Dependencies must
            // be bundled before the script that needs them.
            bundles.Add(new ScriptBundle("~/bundles/smartroutes").Include(
                        // This should always be first.
                        "~/Scripts/View/SmartRoutes.js",
                        
                        // View Models.
                        "~/Scripts/ViewModels/ChildInfoViewModel.js",
                        "~/Scripts/ViewModels/ScheduleTypeViewModel.js",
                        "~/Scripts/ViewModels/AddressViewModel.js",
                        "~/Scripts/ViewModels/DropOffDepartureViewModel.js",
                        "~/Scripts/ViewModels/DropOffDestinationViewModel.js",
                        "~/Scripts/ViewModels/PickUpDepartureViewModel.js",
                        "~/Scripts/ViewModels/PickUpDestinationViewModel.js",
                        "~/Scripts/ViewModels/LocationAndTimeViewModel.js",
                        "~/Scripts/ViewModels/AccreditationViewModel.js",
                        "~/Scripts/ViewModels/ServiceTypeViewModel.js",

                        // Common controllers.
                        "~/Scripts/View/PortalViewController.js",

                        // Search form page controllers.
                        "~/Scripts/View/ChildInformationFormPageController.js",
                        "~/Scripts/View/ScheduleTypeFormPageController.js",
                        "~/Scripts/View/LocationAndTimeFormPageController.js",
                        "~/Scripts/View/AccreditationFormPageController.js",
                        "~/Scripts/View/ServiceTypeFormPageController.js",

                        // The master search form controller.
                        "~/Scripts/View/GuidedSearchViewController.js",

                        // This controller essentially runs the site.
                        "~/Scripts/View/PageController.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/site.css",
                        "~/Content/bootstrap-theme.css",
                        "~/Content/bootstrap.css",
                        "~/Content/Views/MainPageView.css",
                        "~/Content/Views/PortalView.css",
                        "~/Content/Views/GuidedSearchPageView.css"));
        }
    }
}