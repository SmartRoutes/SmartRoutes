/* requires(SmartRoutes.js) */

SmartRoutes.GuidedSearchPageViewController = (function(pageID) {

    // Private:
    
    var guidedSearchPageViewID = pageID;
    var formPageSammyApp = null;
    var activePageElement = null;
    var activePageController = null;
    var formPageFadeInTimeMS = 300;
    var searchCompletedCallback = null;
    var lastSearchPayload = null;

    var buttonIDs = {
        nextButton: "sr-guided-search-button-next",
        previousButton: "sr-guided-search-button-previous",
        resultsButton: "sr-guided-search-button-return-results"
    };

    var elementIDs = {
        searchFormView: "sr-guided-search-form-view",
        breadcrumbView: "sr-guided-search-breadcrumb-view",
        searchContainer: "sr-guided-search-container",
        searchingAnimationContainer: "sr-guided-search-searching-animation-container",
        alert: "sr-location-time-alert"
    };

    var pageIDs = {
        childInformationPageID: "sr-child-information-form-page-view",
        scheduleTypePageID: "sr-schedule-type-form-page-view",
        locationAndTimePageID: "sr-location-time-form-page-view",
        accreditationPageID: "sr-accreditation-form-page-view",
        serviceTypePageID: "sr-service-type-form-page-view"
    };
    
    var pageIDRouteMap = {
        "sr-child-information-form-page-view": "#/search/childinformation",
        "sr-schedule-type-form-page-view": "#/search/scheduletype",
        "sr-location-time-form-page-view": "#/search/locationsandtimes",
        "sr-accreditation-form-page-view": "#/search/accreditation",
        "sr-service-type-form-page-view": "#/search/servicetype"
    };

    var searchResultStatus = {
        ResultsOk: 0,

        DropOffDepartureGeocodeFail: 1,
        DropOffDestinationGeocodeFail: 2,

        PickUpDepartureGeocodeFail: 3,
        PickUpDestinationGecodeFail: 4,
    };

    var childInformationFormPageController = null;
    var scheduleTypeFormPageController = null;
    var locationAndTimeFormPageController = null;
    var accreditationFormPageController = null;
    var serviceTypeFormPageController = null;

    // Handles enabling/disabling the next button when a
    // form page reports that it has invalid data.
    function PageValidationCallbackHandler(pageValid) {
        if (pageValid) {
            $("#" + buttonIDs.nextButton).removeAttr("disabled");
        }
        else {
            $("#" + buttonIDs.nextButton).attr("disabled", "disabled");
        }
    };

    // Handles redirecting to the first form page if a direct URL is used.
    function RedirectToFirstPageIfFirstTimeVisiting() {
        // TODO: this doesn't work like it should at the moment.
        if (!activePageController || !activePageElement) {
            activePageElement = $("#" + pageIDs.childInformationPageID);
            activePageController = childInformationFormPageController;

            // Anywhere else just needs to navigate to #/search.
            // This controller will navigate to the correct sub-route.
            formPageSammyApp.setLocation(pageIDRouteMap[pageIDs.childInformationPageID]);

            // Show the next navigation button.
            $("#" + buttonIDs.nextButton).show();
        }
    };

    // Transitions from the current page to the page controlled by the new controller.
    // Additional arguments are passed to the new controller's RunPage function.
    function TransitionPages(currentPageController, newPageController) {
        RedirectToFirstPageIfFirstTimeVisiting();
        ShowSearchForm();

        activePageController.StopPage();

        $(".sr-form-page").hide();
        $("#" + newPageController.GetFormPageID()).fadeIn(formPageFadeInTimeMS);

        activePageController = newPageController;
        activePageElement = $("#" + activePageController.GetFormPageID());

        var args = Array.prototype.slice.call(arguments);
        var remainingArgs = args.slice(2);

        // Pass the remaining arguments into the new active page.
        activePageController["RunPage"].apply(activePageController, remainingArgs);

        // Setup the navigation buttons based on the active page.
        var nextPages = $(activePageElement).next(".sr-form-page");

        if (nextPages.length === 0) {
            // No remaining pages, change the next button text.
            $("#" + buttonIDs.nextButton).text("Search");
        }
        else {
            // The next button should indicate moving to the next page.
            $("#" + buttonIDs.nextButton).text("Next >");
        }

        // Setup the previous button.
        var previousPages = $(activePageElement).prev(".sr-form-page");
        if (previousPages.length === 0) {
            // No previous page.
            $("#" + buttonIDs.previousButton).hide();
        }
        else {
            $("#" + buttonIDs.previousButton).show();
        }
    };

    // Binds all the routes for the search form pages.
    function InitPageSubroutes() {
        formPageSammyApp = $.sammy(function() {
            this.get(pageIDRouteMap[pageIDs.childInformationPageID], function() {
                TransitionPages(activePageController, childInformationFormPageController, PageValidationCallbackHandler);
            });

            this.get(pageIDRouteMap[pageIDs.scheduleTypePageID], function() {
                TransitionPages(activePageController, scheduleTypeFormPageController, PageValidationCallbackHandler);
            });

            this.get(pageIDRouteMap[pageIDs.locationAndTimePageID], function() {
                var scheduleTypeSelection = scheduleTypeFormPageController.GetScheduleTypeInformationPayload();
                TransitionPages(activePageController, locationAndTimeFormPageController, PageValidationCallbackHandler, scheduleTypeSelection);
            });

            this.get(pageIDRouteMap[pageIDs.accreditationPageID], function() {
                TransitionPages(activePageController, accreditationFormPageController, PageValidationCallbackHandler);
            });

            this.get(pageIDRouteMap[pageIDs.serviceTypePageID], function() {
                TransitionPages(activePageController, serviceTypeFormPageController, PageValidationCallbackHandler);
            });
        });
    };

    // These functions setup the individual pages.

    function InitChildInfoPage() {
        childInformationFormPageController = new SmartRoutes.ChildInformationFormPageController(pageIDs.childInformationPageID);
    };

    function InitScheduleTypePage() {
        scheduleTypeFormPageController = new SmartRoutes.ScheduleTypeFormPageController(pageIDs.scheduleTypePageID);
    };

    function InitLocationAndTimePage() {
        locationAndTimeFormPageController = new SmartRoutes.LocationAndTimeFormPageController(pageIDs.locationAndTimePageID);
    };

    function InitAccreditationPage() {
        accreditationFormPageController = new SmartRoutes.AccreditationFormPageController(pageIDs.accreditationPageID);
    };

    function InitServiceTypeFormPage() {
        serviceTypeFormPageController = new SmartRoutes.ServiceTypeFormPageController(pageIDs.serviceTypePageID);
    };

    (function Init() {
        InitPageSubroutes();
        InitChildInfoPage();
        InitScheduleTypePage();
        InitLocationAndTimePage();
        InitAccreditationPage();
        InitServiceTypeFormPage();
    })();

    // Displays the searching animation and hides the forms.
    function ShowSearchingAnimation() {
        $("#" + elementIDs.searchContainer).hide();
        $("#" + elementIDs.searchingAnimationContainer).show();
    };

    // Shows the search forms and hides the loading animation.
    function ShowSearchForm() {
        $("#" + elementIDs.searchingAnimationContainer).hide();
        $("#" + elementIDs.searchContainer).fadeIn(formPageFadeInTimeMS);
    };

    // Constructs the payload object for the complete search query.
    function CreateChildCareSearchPayload() {
        var childCareSearchPayload = new SmartRoutes.Communication.ChildCareSearchQueryPayload(
                                            childInformationFormPageController.GetChildInformationPayloads(),
                                            scheduleTypeFormPageController.GetScheduleTypeInformationPayload(),
                                            locationAndTimeFormPageController.getLocationAndTimePayload(),
                                            accreditationFormPageController.GetAccreditationPayloadArray(),
                                            serviceTypeFormPageController.GetServiceTypePayloadArray());

        return childCareSearchPayload;
    };

    function HandleSearchError(status) {
        var navPath = "/#";
        if (!status || (status && !status.Code)) {
            alert("Unable to retrieve results from the server");
        }
        else {
            switch (status.Code) {
                case searchResultStatus.DropOffDepartureGeocodeFail:
                case searchResultStatus.DropOffDestinationGeocodeFail:
                case searchResultStatus.PickUpDepartureGeocodeFail:
                case searchResultStatus.PickUpDestinationGecodeFail:
                    locationAndTimeFormPageController.SetErrorFromSearchStatus(status, searchResultStatus);
                    formPageSammyApp.setLocation(pageIDRouteMap[pageIDs.locationAndTimePageID]);
                    break;
                default:
                    alert("An unexpected error occured.");
                    break;
            }
        }
    };

    // Receives data after the server performs a search.
    function SearchCompletedCallback(data) {
        // check data, return to search form or notify the page controller
        // that we have data.
        if (searchCompletedCallback && data && data.Status && (data.Status.Code == searchResultStatus.ResultsOk)) {
            searchCompletedCallback(data, lastSearchPayload);
        }
        else {
            HandleSearchError(data.Status);
        }
    };

    // Initiates the child care search on the server.
    function PerformChildCareSearch() {
        ShowSearchingAnimation();

        var searchPayload = CreateChildCareSearchPayload();
        var guidedSearchCommunicationController = new SmartRoutes.Communication.GuidedSearchCommunicationController();

        lastSearchPayload = searchPayload;

        guidedSearchCommunicationController.PerformChildCareSearch(searchPayload, SearchCompletedCallback);
    };

    // Event handlers

    // Click handler for the previous button.
    $("#" + buttonIDs.previousButton).click(function() {
        var previousPage = $(activePageElement).prev(".sr-form-page");

        if (previousPage.length > 0) {
            // Change the route, this will also change the page.
            var previousPageID = previousPage.attr("id");
            formPageSammyApp.setLocation(pageIDRouteMap[previousPageID]);
        }
    });

    // Click handler for the next button.
    $("#" + buttonIDs.nextButton).click(function() {
        var nextPage = $(activePageElement).next(".sr-form-page");

        if (nextPage.length > 0) {
            var nextPageID = nextPage.attr("id");
            formPageSammyApp.setLocation(pageIDRouteMap[nextPageID]);
        }
        else {
            // No pages left, we should search.
            PerformChildCareSearch();
        }
    });

    return {
        // Public:

        RunPage: function(resultsCallback) {
            RedirectToFirstPageIfFirstTimeVisiting();
            searchCompletedCallback = resultsCallback;
        },

        StopPage: function() {
            // Break this callback.
            searchCompletedCallback = null;
        },

        GetPageViewID: function() {
            return guidedSearchPageViewID;
        },

        GetChildNames: function() {
            return childInformationFormPageController.GetChildNames();
        }
    };
});