
// Controller that handles communicating with the server for
// results page requests.
SmartRoutes.Communication.ResultsCommunicationController = new function() {
    // Private:

    // The path to the results page controller.
    var resultsPageControllerPath = "/ResultsPage/";

    // Map of all requests to their server paths.
    var requestMap = {
        ResultViewHtml: resultsPageControllerPath + "ResultViewHtml"
    };

    return {
        // Public:

        // Fetches the result view HTML from the server.
        FetchResultViewHtml: function(callback) {
            $.get(requestMap.ResultViewHtml, function(data) {
                callback(data);
            });
        }
    };
};