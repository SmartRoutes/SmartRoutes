﻿using System.Web;
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
                        "~/Scripts/Models/ChildInfoViewModel.js",
                        "~/Scripts/Models/ScheduleTypeViewModel.js",
                        "~/Scripts/Models/AddressViewModel.js",
                        "~/Scripts/Models/DropOffDepartureViewModel.js",
                        "~/Scripts/Models/DropOffDestinationViewModel.js",
                        "~/Scripts/Models/PickUpDepartureViewModel.js",
                        "~/Scripts/Models/PickUpDestinationViewModel.js",
                        "~/Scripts/Models/LocationAndTimeViewModel.js",

                        // Common controllers.
                        "~/Scripts/View/PortalViewController.js",

                        // Search form page controllers.
                        "~/Scripts/View/ChildInformationFormPageController.js",
                        "~/Scripts/View/ScheduleTypeFormPageController.js",
                        "~/Scripts/View/LocationAndTimeFormPageController.js",

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

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            //            "~/Scripts/jquery-ui-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.unobtrusive*",
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            //            "~/Content/themes/base/jquery.ui.core.css",
            //            "~/Content/themes/base/jquery.ui.resizable.css",
            //            "~/Content/themes/base/jquery.ui.selectable.css",
            //            "~/Content/themes/base/jquery.ui.accordion.css",
            //            "~/Content/themes/base/jquery.ui.autocomplete.css",
            //            "~/Content/themes/base/jquery.ui.button.css",
            //            "~/Content/themes/base/jquery.ui.dialog.css",
            //            "~/Content/themes/base/jquery.ui.slider.css",
            //            "~/Content/themes/base/jquery.ui.tabs.css",
            //            "~/Content/themes/base/jquery.ui.datepicker.css",
            //            "~/Content/themes/base/jquery.ui.progressbar.css",
            //            "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}