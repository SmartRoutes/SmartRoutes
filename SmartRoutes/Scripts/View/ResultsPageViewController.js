
SmartRoutes.ResultsPageViewController = function() {
    // Private:
    var resultViewHtml = null;
    var resultData = null;
    var resultPageViewCommunicationController = new SmartRoutes.Communication.ResultsCommunicationController();

    function FetchResultViewHtmlCallback(data) {
        resultViewHtml = data;
    };

    function PopulateResultListView() {
        
    }

    (function Init() {
        // We need to retrieve the result view HTML.
        resultPageViewCommunicationController.FetchResultViewHtml(FetchResultViewHtmlCallback);
    })();

    return {
        // Public:

        runpage: function(searchresults) {
            // This will need the results from the server to display.
            resultData = searchresults;
        }
    };
};