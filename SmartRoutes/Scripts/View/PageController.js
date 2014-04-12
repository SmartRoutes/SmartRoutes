/* requires(SmartRousummariestes.js, sammy-0.7.4.js) */

SmartRoutes.pageController = (function () {

    // Private
    var that = this;
    var sammyApp = null;
    var activePageController = null;
    var pageTransitionFadeInTimeMS = 300;
    var lastSearchResults = null;
    var lastSearchQuery = null;
    var lastSearchResultsKey = "last-search-results";
    var lastSearchQueryKey = "last-search-query";
    var activePageURL = null;
    var activePageURLKey = "SRActivePageBaseURL";

    var pageURLs = {
        root: "#/",
        search: "#/search",
        results: "#/results",
        itinerary: "#/itinerary",
        feedback: "#/feedback"
    };

    var pageIDs = {
        mainPage: "sr-main-page-view",
        guidedSearchPage: "sr-guided-search-page-view",
        resultsPage: "sr-results-page-view",
        itineraryPage: "sr-itinerary-page-view"
    };

    var elementIDs = {
        topBannerView: "sr-top-banner-view",
        bottomBannerView: "sr-bottom-banner-view",
        topBannerSpacer: "sr-content-view-spacer-top",
        bottomBannerSpacer: "sr-content-view-spacer-bottom",
    };

    var mainPageViewController = new SmartRoutes.MainPageViewController(pageIDs.mainPage);
    var guidedSearchPageViewController = new SmartRoutes.GuidedSearchPageViewController(pageIDs.guidedSearchPage);
    var resultsPageViewController = new SmartRoutes.ResultsPageViewController(pageIDs.resultsPage);
    var itineraryPageViewController = new SmartRoutes.ItineraryPageViewController(pageIDs.itineraryPage);

    // Callback for storing data when the page refreshes.
    $(window).bind("beforeunload", function() {
        window.sessionStorage.setItem(lastSearchResultsKey, JSON.stringify(lastSearchResults));
        window.sessionStorage.setItem(lastSearchQueryKey, JSON.stringify(lastSearchQuery));
    });

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
        this.get(pageURLs.root, function() {
            TransitionPages(mainPageViewController);
            activePageURL = pageURLs.root;
        });

        // Search page route.
        this.get(pageURLs.search, function() {
            TransitionPages(guidedSearchPageViewController, SearchCompletedCallback);
            activePageURL = pageURLs.search;
        });

        this.get(pageURLs.results, function() {
            TransitionPages(resultsPageViewController, lastSearchResults, lastSearchQuery);
            activePageURL = pageURLs.results;
        });

        this.get(pageURLs.itinerary, function() {
            var childNames = guidedSearchPageViewController.GetChildNames();
            TransitionPages(itineraryPageViewController, lastSearchResults, childNames);
            activePageURL = pageURLs.itinerary;
        });

        this.get(pageURLs.feedback, function() {
            // This logic is a bit more complicated.
            // The feedback form needs to overlay the page.
        });
    });

    $(document).ready(function() {
        SmartRoutes.pageController.HideAllPages();

        lastSearchResults = JSON.parse(window.sessionStorage.getItem(lastSearchResultsKey));
        lastSearchQuery = JSON.parse(window.sessionStorage.getItem(lastSearchQueryKey));

        var lastActivePageURL = window.sessionStorage.getItem(activePageURLKey);

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
        },

        HasActivePage: function() {
            return activePageController !== null;
        },

        IsActivePageSearch: function() {
            return activePageController === guidedSearchPageViewController;
        },

        GetContentAreaHeight: function() {
            // Spacers
            var topBannerSpacer = $("#" + elementIDs.topBannerSpacer);
            var bottomBannerSpacer = $("#" + elementIDs.bottomBannerSpacer);

            var height = window.innerHeight - topBannerSpacer.outerHeight(true) - bottomBannerSpacer.outerHeight(true);
            return height;
        },

        ShowGuidedSearchLoadingAnimation: function() {
            guidedSearchPageViewController.ShowLoadingAnimation();
        },
    };
})();
