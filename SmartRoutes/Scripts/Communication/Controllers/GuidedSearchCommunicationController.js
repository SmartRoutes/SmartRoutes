
SmartRoutes.Communication.GuidedSearchCommunicationController = function() {

    var guidedSearchControllerPath = "/GuidedSearchPage/";

    var requestMap = {
        accreditations: guidedSearchControllerPath + "Accreditations",
        serviceTypes: guidedSearchControllerPath + "ServiceTypes",
        accreditationView: guidedSearchControllerPath + "AccreditationView",
        serviceTypeView: guidedSearchControllerPath + "ServiceTypeView"
    };

    return {

        // Fetches the accreditation data from the server and 
        // calls callback with an array of accreditation model objects.
        FetchAccreditations: function(callback) {
            $.getJSON(requestmap.accreditations, function(data) {
                callback(data);
            });
        },

        FetchAccreditationView: function(callback) {
            $.get("/GuidedSearchPage/AccreditationView", function(data) {
                callback(data);
            }, "html");
        }
    };
};