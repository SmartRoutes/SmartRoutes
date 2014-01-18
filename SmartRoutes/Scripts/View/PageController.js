/* requires(SmartRoutes.js, sammy-0.7.4.js) */

SmartRoutes.pageController = (function () {

    // Private
    var that = this;
    var mainPageID = "sr-main-page-view";
    var guidedSearchID = "sr-guided-search-view";
    var sammyApp = null;

    // Sammy wants the element to operate on and a function
    // that defines the routes.
    this.sammyApp = $.sammy(function() {
        // Setup the routes.

        // Main page route.
        this.get("#/", function() {
            SmartRoutes.pageController.HideAllPages();
            $("#sr-main-page-view").show();
        });

        // Search page route.
        this.get("#/search", function() {
            SmartRoutes.pageController.HideAllPages();
            $("#sr-guided-search-page-view").show();
        });

        this.get("#/results", function() {
            SmartRoutes.pageController.HideAllPages();
            $("#sr-results-page-view").show();
        });

        this.get("#/plan", function() {
            SmartRoutes.pageController.HideAllPages();
            $("#sr-plan-page-view").show();
        });

        this.get("#/feedback", function() {
            // This logic is a bit more complicated.
            // The feedback form needs to overlay the page.
        });
    });

    $(document).ready(function() {
        SmartRoutes.pageController.HideAllPages();
        $("#sr-main-page-view").show();

        // The sammy app should only be run after the document is ready.
        that.sammyApp.run();
    });

    return {
        // Public
        HideAllPages: function() {
            // Maybe need to do more fine-grained hiding to animate it?
            $(".sr-page-view").hide();
        }
    };
})();
