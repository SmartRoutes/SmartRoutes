/* requires(SmartRoutes.js) */

SmartRoutes.GuidedSearchViewController = (function() {

    // Private:
    
    var formPageSammyApp = null;
    var activePageElement = null;
    var activePageController = null;
    var formPageFadeInTimeMS = 300;

    var buttonIDs = {
        nextButton: "sr-guided-search-button-next",
        previousButton: "sr-guided-search-button-previous",
        resultsButton: "sr-guided-search-button-return-results"
    };

    var elementIDs = {
        searchFormView: "sr-guided-search-form-view",
        breadcrumbView: "sr-guided-search-breadcrumb-view",
        searchContainer: "sr-guided-search-container",
        searchingAnimationContainer: "sr-guided-search-searching-animation-container"
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
    }

    var childInformationFormPageController = null;
    var scheduleTypeFormPageController = null;
    var locationAndTimeFormPageController = null;
    var accreditationFormPageController = null;
    var serviceTypeFormPageController = null;

    function PageValidationCallbackHandler(pageValid) {
        if (pageValid) {
            $("#" + buttonIDs.nextButton).removeAttr("disabled");
        }
        else {
            $("#" + buttonIDs.nextButton).attr("disabled", "disabled");
        }
    };

    function RedirectToFirstPageIfFirstTimeVisiting() {
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

    function ShowSearchingAnimation() {
        $("#" + elementIDs.searchContainer).hide();
        $("#" + elementIDs.searchingAnimationContainer).show();
    };

    function ShowSearchForm() {
        $("#" + elementIDs.searchingAnimationContainer).hide();
        $("#" + elementIDs.searchContainer).fadeIn(formPageFadeInTimeMS);
    };

    function CreateChildCareSearchPayload() {
        var childCareSearchPayload = new SmartRoutes.Communication.ChildCareSearchQueryPayload(
                                            childInformationFormPageController.GetChildInformationPayloads(),
                                            scheduleTypeFormPageController.GetScheduleTypeInformationPayload(),
                                            accreditationFormPageController.GetAccreditationPayloadArray(),
                                            serviceTypeFormPageController.GetServiceTypePayloadArray());

        return childCareSearchPayload;
    };

    function SearchCompletedCallback(data) {
        // check data, return to search form or notify the page controller
        // that we have data.
    };

    function PerformChildCareSearch() {
        ShowSearchingAnimation();

        var searchPayload = CreateChildCareSearchPayload();
        var guidedSearchCommunicationController = new SmartRoutes.Communication.GuidedSearchCommunicationController();

        //guidedSearchCommunicationController.PerformChildCareSearch(searchPayload, SearchCompletedCallback);
    };

    // Event handlers

    $("#" + buttonIDs.previousButton).click(function() {
        var previousPage = $(activePageElement).prev(".sr-form-page");

        if (previousPage.length > 0) {
            // Change the route, this will also change the page.
            var previousPageID = previousPage.attr("id");
            formPageSammyApp.setLocation(pageIDRouteMap[previousPageID]);
        }
    });

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

        RunPage: function() {
            RedirectToFirstPageIfFirstTimeVisiting();
        }
    };
});