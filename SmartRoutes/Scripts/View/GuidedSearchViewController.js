/* requires(SmartRoutes.js) */

SmartRoutes.guidedSearchViewController = (function() {

    // Private:
    var maxChildren = 3;
    var childCount = 1;
    var childInfoViewModels = new Array();
    var formPageSammyApp = null;
    var activePageElement = null;
    
    var pageIDRouteMap = {
        "sr-child-information-form-page-view": "#/search/childinformation",
        "sr-schedule-type-form-page-view": "#/search/scheduletype"
    }

    var InitPageSubroutes = function() {
        formPageSammyApp = $.sammy(function() {
            // This section should just handle page manipulations.
            // Validation of whether the navigation should be allowed 
            // should be handled elsewhere.

            this.get(pageIDRouteMap["sr-child-information-form-page-view"], function() {
                $(".sr-form-page").hide();
                $("#sr-child-information-form-page-view").show();
            });

            this.get(pageIDRouteMap["sr-schedule-type-form-page-view"], function() {
                $(".sr-form-page").hide();
                $("#sr-schedule-type-form-page-view").show();
            });
        });
    };

    var InitChildInfoPage = function() {
        // Setup the expansion button click handler.
        $(".sr-expansion-button").click(function() {
            // So, this callback is hit for every expansion button.
            // What must happen is
            // 1) Show the next section (if it exists).
            // 2) Change the expansion button to a collapse button
            // the current child info section.

            var nextChildInfoElement = $(this).closest(".sr-child-info-view").next();
            if (nextChildInfoElement.length > 0) {
                $(".sr-expansion-button").hide();
                $(".sr-collapse-button").hide();

                $(this).next("button", ".sr-collapse-button").show();
                $(".sr-expansion-button", nextChildInfoElement).show();

                nextChildInfoElement.show();
                ++childCount;
            }
        });

        // Setup the collapse button click handler.
        $(".sr-collapse-button").click(function() {
            // This is essentially the reverse of the
            // expansion button handler.

            var nextChildInfoElement = $(this).closest(".sr-child-info-view").next();
            nextChildInfoElement.hide();
            --childCount;

            // Hide the buttons.
            $(".sr-expansion-button").hide();
            $(".sr-collapse-button").hide();

            // Show the expansion button on the current child info view.
            $(this).prev("button", ".sr-expand-button").show();

            // Look at the previous child info view and show its
            // collapse button if the view exists.
            var previousChildInfoElement = $(this).closest(".sr-child-info-view").prev();
            if (previousChildInfoElement.length > 0) {
                $(".sr-collapse-button", previousChildInfoElement).show();
            }
        });

        // Setup the knockout viewmodel bindings.
        var childInfoViews = $(".sr-child-info-view");
        for (var childInfoIndex = 0; childInfoIndex < childInfoViews.length; ++childInfoIndex) {
            childInfoViewModels[childInfoIndex] = new ChildInfoViewModel();
            ko.applyBindings(childInfoViewModels[childInfoIndex], childInfoViews[childInfoIndex]);
        }
    };

    var InitScheduleTypePage = function() {

    };

    (function Init() {
        InitPageSubroutes();
        InitChildInfoPage();
        InitScheduleTypePage();
    })();

    // Event handlers

    $("#sr-guided-search-button-previous").click(function() {
        var previousPage = $(activePageElement).prev().find(".sr-form-page");

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

        GetChildInfoViewModels: function() {
            return childInfoViewModels;
        },

        RunPageSubroutes: function() {
            // Anywhere else just needs to navigate to #/search.
            // This controller will navigate to the correct sub-route.
            formPageSammyApp.setLocation(pageIDRouteMap["sr-child-information-form-page-view"]);
            activePageElement = $("#sr-child-information-form-page-view");
        }
    };
})();