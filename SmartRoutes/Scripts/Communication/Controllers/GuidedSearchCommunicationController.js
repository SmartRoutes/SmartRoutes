
// Controller that handles communication with the server
// for guided search requests.
SmartRoutes.Communication.GuidedSearchCommunicationController = function() {
    // Private:

    // Path to the guided search controller.
    var guidedSearchControllerPath = "/GuidedSearchPage/";

    // Map containing all requests that can be made and their paths.
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
                && childCareSearchQueryPayload.ChildInformation
                && childCareSearchQueryPayload.ScheduleType
                && childCareSearchQueryPayload.Accreditations
                && childCareSearchQueryPayload.ServiceTypes) {
                $.ajax({
                    url: requestMap.performChildCareSearch,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(childCareSearchQueryPayload),
                    dataType: "json",
                    success: function(data) {
                        if (data) {
                            callback(data);
                        }
                    }
                });
            }
        }
    };
};