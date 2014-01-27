/* requires(SmartRoutes.js) */

SmartRoutes.GuidedSearchViewController = (function() {

    // Private:
    
    var formPageSammyApp = null;
    var activePageElement = null;
    var activePageController = null;

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

    var InitPageSubroutes = function() {
        formPageSammyApp = $.sammy(function() {
            // TODO: Validation should be handled when the Next button is clicked.
            // However, we also need to prevent navigating directly to a URL.
            // If the navigation wasn't done with the Next button, it might make sense
            // to simply dump the user back on the first search form page?

            this.get(pageIDRouteMap[pageIDs.childInformationPageID], function() {
                RedirectToFirstPageIfFirstTimeVisiting();

                activePageController.StopPage();
                $(".sr-form-page").hide();
                activePageController = childInformationFormPageController;
                activePageElement = $("#" + pageIDs.childInformationPageID);
                activePageController.RunPage();
            });

            this.get(pageIDRouteMap[pageIDs.scheduleTypePageID], function() {
                RedirectToFirstPageIfFirstTimeVisiting();

                activePageController.StopPage();
                $(".sr-form-page").hide();
                activePageController = scheduleTypeFormPageController;
                activePageElement = $("#" + pageIDs.scheduleTypePageID);
                activePageController.RunPage(PageValidationCallbackHandler);
            });

            this.get(pageIDRouteMap[pageIDs.locationAndTimePageID], function() {
                RedirectToFirstPageIfFirstTimeVisiting();

                var scheduleTypeSelection = activePageController.GetScheduleTypeInformation();

                $(".sr-form-page").hide();
                activePageController = locationAndTimeFormPageController;
                activePageElement = $("#" + pageIDs.locationAndTimePageID);
                activePageController.RunPage(PageValidationCallbackHandler, scheduleTypeSelection);
            });

            this.get(pageIDRouteMap[pageIDs.accreditationPageID], function() {
                RedirectToFirstPageIfFirstTimeVisiting();

                $(".sr-form-page").hide();
                $("#" + pageIDs.accreditationPageID).show();
            });

            this.get(pageIDRouteMap[pageIDs.serviceTypePageID], function() {
                RedirectToFirstPageIfFirstTimeVisiting();

                $(".sr-form-page").hide();
                $("#" + pageIDs.serviceTypePageID).show();
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
        locationAndTimeFormPageController = new SmartRoutes.LocationAndTimeFormPageController(pageIDRouteMap);
    };

    var InitAccreditationPage = function() {
        accreditationFormPageController = new SmartRoutes.AccreditationFormPageController();
    };

    var InitServiceTypeFormPage = function() {
        serviceTypeFormPageController = new SmartRoutes.ServiceTypeFormPageController();
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