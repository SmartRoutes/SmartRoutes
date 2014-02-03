
SmartRoutes.Communication.GuidedSearchCommunicationController = function() {
    // Private:

    var guidedSearchControllerPath = "/GuidedSearchPage/";

    var requestMap = {
        accreditations: guidedSearchControllerPath + "Accreditations",
        serviceTypes: guidedSearchControllerPath + "ServiceTypes",
        accreditationView: guidedSearchControllerPath + "AccreditationView",
        serviceTypeView: guidedSearchControllerPath + "ServiceTypeView"
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

        FetchAccreditationView: function(callback) {
            $.get(requestMap.accreditationView, function(data) {
                callback(data);
            }, "html");
        },

        FetchServiceTypes: function(callback) {
            $.getJSON(requestMap.serviceTypes, function(data) {
                callback(data);
            });
        },

        FetchServiceTypeView: function(callback) {
            $.get(requestMap.serviceTypeView, function(data) {
                callback(data);
            }, "html");
        }
    };
};