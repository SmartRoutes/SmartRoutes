
SmartRoutes.ResultsPageViewController = function(pageID) {
    // Private:
    var resultsPageViewID = pageID;
    var resultViewHtml = null;
    var resultData = null;
    var resultPageViewCommunicationController = new SmartRoutes.Communication.ResultsCommunicationController();

    var resultsListViewModel = new SmartRoutes.ResultListViewViewModel();;
    var query = null;
    var results = null;

    var elementIDs = {
        resultsListView: "sr-results-list-view",
        routeSummaryTemplate: "sr-results-routes-summary-template"
    };

    var elementClasses = {
        resultListElement: "sr-results-list-element",
        routeSummaryContainer: "sr-results-route-summary-routes-container",
        dropOffSummarycontainer: "sr-results-drop-off-route-summary-container",
        pickUpSummaryContainer: "sr-results-pick-up-route-summary-container"
    };

    // Receives the result list view from the server. 
    function FetchResultListViewElementHtmlCallback(data) {
        resultViewHtml = data;
    };

    // Clears and repopulates the result list view with result data.
    function PopulateResultListView(searchResults) {
        resultsListViewModel.SetNewResults(searchResults);

        ko.applyBindings({
            results: resultsListViewModel.elements
        }, $("#" + elementIDs.resultsListView)[0]);

        // Setup the route summary section.
        var templateSource = $("#" + elementIDs.routeSummaryTemplate).html();
        var template = Handlebars.compile(templateSource);
        var resultsListElements = $("." + elementClasses.resultListElement);
        $.each(resultsListViewModel.elements(), function(index, value) {
            // Add the drop off route summary.
            $("." + elementClasses.dropOffSummarycontainer,
              resultsListElements[index]).html(template({
                  routes: value.dropOffRoutes
              }));

            // Add the pick up route summary.
            $("." + elementClasses.pickUpSummaryContainer,
              resultsListElements[index]).html(template({
                  routes: value.pickUpRoutes
              }));
        });
    };

    (function Init() {
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