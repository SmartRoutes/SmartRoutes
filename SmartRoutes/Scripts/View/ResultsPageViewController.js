
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
        resultsListView: "sr-results-list-view"
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
    };

    (function Init() {
        // We need to retrieve the result view HTML.
        //resultPageViewCommunicationController.FetchResultListViewElementHtml(FetchResultListViewElementHtmlCallback);
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