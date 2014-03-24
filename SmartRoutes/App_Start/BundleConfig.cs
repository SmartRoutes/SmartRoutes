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
                        "~/Scripts/library/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/library/knockout-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/library/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/routing").Include(
                        "~/Scripts/library/sammy-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/handlebars").Include(
                        "~/Scripts/library/handlebars-v1.3.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/leaflet").Include(
                        "~/Scripts/library/leaflet-{version}.js"));

            // Initialization order matters here.  Dependencies must
            // be bundled before the script that needs them.
            bundles.Add(new ScriptBundle("~/bundles/smartroutes").Include(
                        // This should always be first.
                        "~/Scripts/SmartRoutes.js",

                        "~/Scripts/Constants.js",

                        // Communication objects.
                        "~/Scripts/Communication/Payloads/AddressPayload.js",
                        "~/Scripts/Communication/Payloads/LocationsAndTimesPayload.js",
                        "~/Scripts/Communication/Payloads/ChildInformationPayload.js",
                        "~/Scripts/Communication/Payloads/ScheduleTypePayload.js",
                        "~/Scripts/Communication/Payloads/AccreditationPayload.js",
                        "~/Scripts/Communication/Payloads/ServiceTypePayload.js",
                        "~/Scripts/Communication/Payloads/ChildCareSearchQueryPayload.js",

                        // Validation.
                        "~/Scripts/Utility/FormValidator.js",

                        // Communication controllers.
                        "~/Scripts/Communication/Controllers/GuidedSearchCommunicationController.js",
                        "~/Scripts/Communication/Controllers/ResultsCommunicationController.js",
                        "~/Scripts/Communication/Controllers/ItineraryCommunicationController.js",

                        // Common/Generic view controllers.
                        "~/Scripts/View/DetailedCheckboxViewController.js",

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
                        "~/Scripts/ViewModels/ResultListViewViewModel.js",
                        "~/Scripts/ViewModels/ChildCareDescriptionViewModel.js",

                        // Common controllers.
                        "~/Scripts/View/PortalViewController.js",

                        // Search form page controllers.
                        "~/Scripts/View/Search/ChildInformationFormPageController.js",
                        "~/Scripts/View/Search/ScheduleTypeFormPageController.js",
                        "~/Scripts/View/Search/LocationAndTimeFormPageController.js",
                        "~/Scripts/View/Search/AccreditationFormPageController.js",
                        "~/Scripts/View/Search/ServiceTypeFormPageController.js",

                        // Page Controllers.
                        "~/Scripts/View/MainPageViewController.js",
                        "~/Scripts/View/Search/GuidedSearchPageViewController.js",
                        "~/Scripts/View/ResultsPageViewController.js",
                        "~/Scripts/View/ItineraryPageViewController.js",

                        // This controller essentially runs the site.
                        "~/Scripts/View/PageController.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap-theme.css",
                        "~/Content/bootstrap.css",
                        "~/Content/leaflet-{version}.css",
                        "~/Content/site.css",
                        "~/Content/Views/GenericControls.css",
                        "~/Content/Views/MainPageView.css",
                        "~/Content/Views/PortalView.css",
                        "~/Content/Views/GuidedSearchPageView.css",
                        "~/Content/Views/ResultsPageView.css",
                        "~/Content/Views/ItineraryPageView.css"));
        }
    }
}