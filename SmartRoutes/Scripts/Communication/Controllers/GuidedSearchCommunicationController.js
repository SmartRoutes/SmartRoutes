
SmartRoutes.Communication.GuidedSearchCommunicationController = function() {
    // Private:

    var guidedSearchControllerPath = "/GuidedSearchPage/";

    var requestMap = {
        accreditations: guidedSearchControllerPath + "Accreditations",
        serviceTypes: guidedSearchControllerPath + "ServiceTypes",
        accreditationView: guidedSearchControllerPath + "AccreditationView",
        serviceTypeView: guidedSearchControllerPath + "ServiceTypeView",
        performChildCareSearch: guidedSearchControllerPath + "PerformChildCareSearch"
    };

    return {
        // Public:

        // Fetches the accreditation data from the server and 
        // calls callback with an array of accreditation model objects.
        FetchAccreditations: function(callback) {
            $.getJSON(requestMap.accreditations, function(data) {
                callback(data);
            });
        },

        // Fetches the raw html for the accreditation view from the server.
        FetchAccreditationView: function(callback) {
            $.get(requestMap.accreditationView, function(data) {
                callback(data);
            }, "html");
        },

        // Fetches the JSON data for service types from the server.
        FetchServiceTypes: function(callback) {
            $.getJSON(requestMap.serviceTypes, function(data) {
                callback(data);
            });
        },

        // Fetches the raw html for the service type view from the server.
        FetchServiceTypeView: function(callback) {
            $.get(requestMap.serviceTypeView, function(data) {
                callback(data);
            }, "html");
        },

        // Sends the child care search query payload to the server to perform
        // a search.  The payload must contain all required properties.
        PerformChildCareSearch: function(childCareSearchQueryPayload, callback) {
            if (childCareSearchQueryPayload
                && childCareSearchQueryPayload.childInformation
                && childCareSearchQueryPayload.scheduleType
                && childCareSearchQueryPayload.accreditations
                && childCareSearchQueryPayload.serviceTypes) {
                $.post(requestMap.performChildCareSearch, childCareSearchQueryPayload, function(data) {
                    callback(data);
                }, "json");
            }
        }
    };
};