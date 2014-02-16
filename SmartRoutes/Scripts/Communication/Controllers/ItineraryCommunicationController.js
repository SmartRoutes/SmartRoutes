
SmartRoutes.Communication.ItineraryCommunicationController = function() {
    // Private:
    
    var itineraryPageControllerPath = "/ItineraryPage/";

    var requestMap = {
        fetchChildCareServiceDescriptionView: itineraryPageControllerPath + "FetchChildCareServiceDescriptionViewHTML"
    };

    return {
        // Public:

        FetchChildCareServiceDescriptionViewHTML: function(htmlRetrievedCallback) {
            $.get(requestMap.fetchChildCareServiceDescriptionView, function(data) {
                if (htmlRetrievedCallback) {
                    htmlRetrievedCallback(data);
                }
            }, "html");
        }
    };
};