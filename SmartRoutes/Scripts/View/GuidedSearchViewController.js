/* requires(SmartRoutes.js) */

SmartRoutes.GuidedSearchViewController = (function() {

    // Private:
    
    var formPageSammyApp = null;
    var activePageElement = null;
    
    var pageIDRouteMap = {
        "sr-child-information-form-page-view": "#/search/childinformation",
        "sr-schedule-type-form-page-view": "#/search/scheduletype",
        "sr-location-time-form-page-view": "#/search/locationsandtimes",
        "sr-accreditation-form-page-view": "#/search/accreditation"
    }
    var childInformationFormPageController = null;
    var scheduleTypeFormPageController = null;
    var locationAndTimeFormPageController = null;

    var InitPageSubroutes = function() {
        formPageSammyApp = $.sammy(function() {
            // TODO: Validation should be handled when the Next button is clicked.
            // However, we also need to prevent navigating directly to a URL.
            // If the navigation wasn't done with the Next button, it might make sense
            // to simply dump the user back on the first search form page?

            this.get(pageIDRouteMap["sr-child-information-form-page-view"], function() {
                $(".sr-form-page").hide();
                $("#sr-child-information-form-page-view").show();
            });

            this.get(pageIDRouteMap["sr-schedule-type-form-page-view"], function() {
                $(".sr-form-page").hide();
                $("#sr-schedule-type-form-page-view").show();
            });

            this.get(pageIDRouteMap["sr-location-time-form-page-view"], function() {
                $(".sr-form-page").hide();
                $("#sr-location-time-form-page-view").show();
            });

            this.get(pageIDRouteMap["sr-accreditation-form-page-view"], function() {
                $(".sr-form-page").hide();
                $("#sr-accreditation-form-page-view").show();
            });
        });
    };

    var InitChildInfoPage = function() {
        childInformationFormPageController = new SmartRoutes.ChildInformationFormPageController(pageIDRouteMap);
    };

    var InitScheduleTypePage = function() {
        scheduleTypeFormPageController = new SmartRoutes.ScheduleTypeFormPageController(pageIDRouteMap);
    };

    var InitLocationAndTimePage = function() {
        locationAndTimeFormPageController = new SmartRoutes.LocationAndTimeFormPageController(pageIDRouteMap);
    };

    (function Init() {
        InitPageSubroutes();
        InitChildInfoPage();
        InitScheduleTypePage();
        InitLocationAndTimePage();
    })();

    // Event handlers

    $("#sr-guided-search-button-previous").click(function() {
        var previousPage = $(activePageElement).prev(".sr-form-page");

        if (previousPage.length > 0) {
            // Change the route, this will also change the page.
            var previousPageID = previousPage.attr("id");
            formPageSammyApp.setLocation(pageIDRouteMap[previousPageID]);

            activePageElement = previousPage;
        }
    });

    $("#sr-guided-search-button-next").click(function() {
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
            // Anywhere else just needs to navigate to #/search.
            // This controller will navigate to the correct sub-route.
            formPageSammyApp.setLocation(pageIDRouteMap["sr-child-information-form-page-view"]);
            activePageElement = $("#sr-child-information-form-page-view");
        }
    };
});