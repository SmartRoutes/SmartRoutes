
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

    // Inserts result view HTML into the list view and binds it to view models
    function InsertAndBindResultHTML() {
        ko.applyBindings({
            results: resultsListViewModel.elements
        }, $("#" + elementIDs.resultsListView)[0]);


    };


    // Clears and repopulates the result list view with result data.
    function PopulateResultListView(searchResults) {
        InsertAndBindResultHTML(resultsListViewViewModels);

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
        },

        StopPage: function() {

        },

        GetPageViewID: function() {
            return resultsPageViewID;
        }
    };
};