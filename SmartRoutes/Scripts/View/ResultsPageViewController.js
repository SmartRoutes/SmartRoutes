
SmartRoutes.ResultsPageViewController = function() {
    // Private:
    var resultViewHtml = null;
    var resultData = null;
    var resultPageViewCommunicationController = new SmartRoutes.Communication.ResultsCommunicationController();

    function FetchResultListViewElementHtmlCallback(data) {
        resultViewHtml = data;
    };

    function PopulateResultListView() {
        // TODO:
        // Iterate over the results to
        // 1) Transform the results into view model objects
        // 2) insert the HTML and 
        // 3) Bind the html to view models

    }

    (function Init() {
        // We need to retrieve the result view HTML.
        resultPageViewCommunicationController.FetchResultListViewElementHtml(FetchResultListViewElementHtmlCallback);
    })();

    return {
        // Public:

        runpage: function(searchresults) {
            // This will need the results from the server to display.
            resultData = searchresults;
        }
    };
};