/* requires(SmartRousummariestes.js, sammy-0.7.4.js) */

SmartRoutes.pageController = (function () {

    // Private
    var that = this;
    var sammyApp = null;
    var activePageController = null;
    var pageTransitionFadeInTimeMS = 300;
    var lastSearchResults = null;
    var lastSearchQuery = null;

    var pageIDs = {
        mainPage: "sr-main-page-view",
        guidedSearchPage: "sr-guided-search-page-view",
        resultsPage: "sr-results-page-view",
        itineraryPage: "sr-itinerary-page-view"
    };

    var mainPageViewController = new SmartRoutes.MainPageViewController(pageIDs.mainPage);
    var guidedSearchPageViewController = new SmartRoutes.GuidedSearchPageViewController(pageIDs.guidedSearchPage);
    var resultsPageViewController = new SmartRoutes.ResultsPageViewController(pageIDs.resultsPage);
    var itineraryPageViewController = new SmartRoutes.ItineraryPageViewController(pageIDs.itineraryPage);

    // Handles transitioning from one page to another.
    function TransitionPages(newPageController) {
        if (activePageController) {
            activePageController.StopPage();
        }

        $(".sr-page-view").hide();
        $("#" + newPageController.GetPageViewID()).fadeIn(pageTransitionFadeInTimeMS);

        activePageController = newPageController;

        var args = Array.prototype.slice.call(arguments);
        var remainingArgs = args.slice(1);

        activePageController["RunPage"].apply(activePageController, remainingArgs);
    };

    // Sammy wants the element to operate on and a function
    // that defines the routes.
    this.sammyApp = $.sammy(function() {
        // Setup the routes.

        // Main page route.
        this.get("#/", function() {
            TransitionPages(mainPageViewController);
        });

        // Search page route.
        this.get("#/search", function() {
            TransitionPages(guidedSearchPageViewController, SearchCompletedCallback);
        });

        this.get("#/results", function() {
            TransitionPages(resultsPageViewController, lastSearchResults, lastSearchQuery);
        });

        this.get("#/itinerary", function() {
            TransitionPages(itineraryPageViewController, lastSearchResults);
        });

        this.get("#/feedback", function() {
            // This logic is a bit more complicated.
            // The feedback form needs to overlay the page.
        });
    });

    $(document).ready(function() {
        SmartRoutes.pageController.HideAllPages();

        // The sammy app should only be run after the document is ready.
        that.sammyApp.run("#/");
    });

    function SearchCompletedCallback(data, searchQuery) {
        if (data) {
            lastSearchResults = data;
            lastSearchQuery = searchQuery;
            that.sammyApp.setLocation("#/results");
        }
        else {
            alert("Unable to retrieve results at this time");
            that.sammyApp.run("#/");
        }
    };

    return {
        // Public
        HideAllPages: function() {
            // Maybe need to do more fine-grained hiding to animate it?
            $(".sr-page-view").hide();
        }
    };
})();
