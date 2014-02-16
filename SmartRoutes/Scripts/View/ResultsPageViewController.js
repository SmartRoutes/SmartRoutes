
SmartRoutes.ResultsPageViewController = function() {
    // Private:
    var resultViewHtml = null;
    var resultData = null;
    var resultPageViewCommunicationController = new SmartRoutes.Communication.ResultsCommunicationController();
    var resultsListViewViewModels = null;

    var elementIDs = {
        resultsListView: "sr-results-list-view"
    };

    // Receives the result list view from the server. 
    function FetchResultListViewElementHtmlCallback(data) {
        resultViewHtml = data;
    };

    // Transforms search results into bindable view models.
    function TransformResultsToViewModels(searchResults) {
        var listViews = new Array();
        // TODO: Extract data from the results and insert into a view model.
        return listViews;
    };

    // Inserts result view HTML into the list view and binds it to view models
    function InsertAndBindResultHTML(viewModels) {
        // First clear all the container.
        var resultListViewSelector = "#" + elementIDs.resultsListView;
        $(resultListViewSelector).empty();
        for (var resultIndex = 0; resultIndex < viewModels.length; ++resultIndex) {
            $(resultListViewSelector).append(resultViewHtml);
            var insertedNode = $(resultListViewSelector).children().last()[0];
            ko.applyBindings(viewModels[resultIndex], insertedNode);
        }
    };

    // Clears and repopulates the result list view with result data.
    function PopulateResultListView(searchResults) {
        // Iterate over the results to
        // 1) Transform the results into view model objects
        // 2) insert the HTML and 
        // 3) Bind the html to view models
        resultsListViewViewModels = TransformResultsToViewModels(searchResults);
        InsertAndBindResultHTML(resultsListViewViewModels);
    };

    (function Init() {
        // We need to retrieve the result view HTML.
        resultPageViewCommunicationController.FetchResultListViewElementHtml(FetchResultListViewElementHtmlCallback);
    })();

    return {
        // Public:

        RunPage: function(searchresults) {
            // This will need the results from the server to display.
        }
    };
};