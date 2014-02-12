
SmartRoutes.Communication.ResultsCommunicationController = new function() {
    // Private:

    var resultsPageControllerPath = "/ResultsPage/";

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