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
        "sr-service-type-form-page-view": "#/sereach/servicetype"
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
        }
    };

    // Transitions from the current page to the page controlled by the new controller.
    // Additional arguments are passed to the new controller's RunPage function.
    function TransitionPages(currentPageController, newPageController) {
        RedirectToFirstPageIfFirstTimeVisiting();

        activePageController.StopPage();

        $(".sr-form-page").hide();
        $("#" + newPageController.GetFormPageID()).fadeIn(formPageFadeInTimeMS);

        activePageController = newPageController;
        activePageElement = $("#" + activePageController.GetFormPageID());

        var args = Array.prototype.slice.call(arguments);
        var remainingArgs = args.slice(2);

        // Pass the remaining arguments into the new active page.
        activePageController["RunPage"].apply(activePageController, remainingArgs);
    };

    var InitPageSubroutes = function() {
        formPageSammyApp = $.sammy(function() {
            this.get(pageIDRouteMap[pageIDs.childInformationPageID], function() {
                TransitionPages(activePageController, childInformationFormPageController);
            });

            this.get(pageIDRouteMap[pageIDs.scheduleTypePageID], function() {
                TransitionPages(activePageController, scheduleTypeFormPageController, PageValidationCallbackHandler);
            });

            this.get(pageIDRouteMap[pageIDs.locationAndTimePageID], function() {
                var scheduleTypeSelection = scheduleTypeFormPageController.GetScheduleTypeInformation();
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

    var InitChildInfoPage = function() {
        childInformationFormPageController = new SmartRoutes.ChildInformationFormPageController(pageIDs.childInformationPageID);
    };

    var InitScheduleTypePage = function() {
        scheduleTypeFormPageController = new SmartRoutes.ScheduleTypeFormPageController(pageIDs.scheduleTypePageID);
    };

    var InitLocationAndTimePage = function() {
        locationAndTimeFormPageController = new SmartRoutes.LocationAndTimeFormPageController(pageIDs.locationAndTimePageID);
    };

    var InitAccreditationPage = function() {
        accreditationFormPageController = new SmartRoutes.AccreditationFormPageController(pageIDs.accreditationPageID);
    };

    var InitServiceTypeFormPage = function() {
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

    // Event handlers

    $("#" + buttonIDs.previousButton).click(function() {
        var previousPage = $(activePageElement).prev(".sr-form-page");

        if (previousPage.length > 0) {
            // Change the route, this will also change the page.
            var previousPageID = previousPage.attr("id");
            formPageSammyApp.setLocation(pageIDRouteMap[previousPageID]);

            activePageElement = previousPage;
        }
    });

    $("#" + buttonIDs.nextButton).click(function() {
        var nextPage = $(activePageElement).next(".sr-form-page");

        if (nextPage.length > 0) {
            var nextPageID = nextPage.attr("id");
            formPageSammyApp.setLocation(pageIDRouteMap[nextPageID]);

            // TODO: should do page validation here since the user
            // has indicated that they want to move to the next page.
            activePageElement = nextPage;
        }
    });


    return {
        // Public:

        RunPage: function() {
            RedirectToFirstPageIfFirstTimeVisiting();
        }
    };
});