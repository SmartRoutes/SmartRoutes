
SmartRoutes.ResultsPageViewController = function(pageID) {
    // Private:
    var resultsPageViewID = pageID;
    var resultViewHtml = null;
    var resultData = null;
    var resultPageViewCommunicationController = new SmartRoutes.Communication.ResultsCommunicationController();
    var serviceNameSummaryPrefix = "ServiceName:";
    var routeNumberSummaryPrefix = "RouteNumber:";
    var serviceNameIconPath = "Content/Images/ServiceIcon.png";
    var routeNumberIconPath = "Content/Images/RouteIcon.png";

    var resultsListViewModel = new SmartRoutes.ResultListViewViewModel();;
    var query = null;
    var results = null;

    var elementIDs = {
        resultsListView: "sr-results-list-view",
        routeSummaryTemplate: "sr-results-routes-summary-template",
        dropOffSummarycontainer: "sr-results-drop-off-route-summary-container",
        pickUpSummaryContainer: "sr-results-pick-up-route-summary-container"
    };

    var elementClasses = {
        resultListElement: "sr-results-list-element",
        routeSummaryContainer: "sr-results-route-summary-routes-container",
        
    };

    // Receives the result list view from the server. 
    function FetchResultListViewElementHtmlCallback(data) {
        resultViewHtml = data;
    };

    // Handlebars helper that receives a route summary string
    // and returns the image tag for that string.
    function RouteSummaryIconHelper(obj) {
        var iconTag = "<img src=\"";
        if (obj && (typeof obj === "string")) {
            if (obj.slice(0, serviceNameSummaryPrefix.length) == serviceNameSummaryPrefix) {
                iconTag += serviceNameIconPath;
            }
            else if (obj.slice(0, routeNumberSummaryPrefix.length) == routeNumberSummaryPrefix) {
                iconTag += routeNumberIconPath;
            }
        }

        iconTag += "\" class=\"sr-route-summary-icon\"/>";

        return new Handlebars.SafeString(iconTag);
    };

    // Handlbars helper that extracts a route or service name
    // from the special summary string.
    function RouteSummaryTextHelper(obj) {
        var summaryText = "";
        if (obj && (typeof obj === "string")) {
            if (obj.slice(0, serviceNameSummaryPrefix.length) == serviceNameSummaryPrefix) {
                summaryText = obj.substring(serviceNameSummaryPrefix.length);
            }
            else if (obj.slice(0, routeNumberSummaryPrefix.length) == routeNumberSummaryPrefix) {
                summaryText = obj.substring(routeNumberSummaryPrefix.length);
            }
        }

        return new Handlebars.SafeString(summaryText);
    };

    // Clears and repopulates the result list view with result data.
    function PopulateResultListView(searchResults) {
        // Remove any existing bindings.
        ko.cleanNode(("#" + elementIDs.resultsListView)[0]);

        resultsListViewModel.SetNewResults(searchResults);

        // Setup the route summary section.
        var templateSource = $("#" + elementIDs.routeSummaryTemplate).html();
        var template = Handlebars.compile(templateSource);
        var resultsListElements = $("." + elementClasses.resultListElement);
        $.each(resultsListViewModel.elements(), function(index, value) {
            // Add the drop off route summary.
            var dropOffRoute = template({
                routes: value.dropOffRoutes
            })
            $("#" + elementIDs.dropOffSummarycontainer, resultsListElements[index]).append(dropOffRoute);

            // Add the pick up route summary.
            var pickUpRoute = template({
                routes:value.pickUpRoutes
            })
            $("#" + elementIDs.pickUpSummaryContainer, resultsListElements[index]).append(pickUpRoute);
        });
    };

    (function Init() {
        Handlebars.registerHelper("RouteSummaryIconHelper", RouteSummaryIconHelper);
        Handlebars.registerHelper("RouteSummaryTextHelper", RouteSummaryTextHelper);

        ko.applyBindings({
            results: resultsListViewModel.elements
        }, $("#" + elementIDs.resultsListView)[0]);
    })();

    return {
        // Public:

        RunPage: function(searchResults, searchQuery) {
            // This will need the results from the server to display.
            query = searchQuery;
            results = searchResults;
            PopulateResultListView(results);
        },

        StopPage: function() {

        },

        GetPageViewID: function() {
            return resultsPageViewID;
        }
    };
};