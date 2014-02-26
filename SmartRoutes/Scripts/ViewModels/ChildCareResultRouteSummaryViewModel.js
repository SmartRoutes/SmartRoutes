
SmartRoutes.ChildCareResultRouteSummaryViewModel = function(itineraryModel) {
    // Private:

    var model = itineraryModel;
    var type = null;
    var routeSummary = null;

    var routeTypeText = [
        "Drop Off",
        "Pick Up"
    ];

    (function Init() {
        if (model.Type > 0 && model.Type > routeTypeText.length) {
            type = ko.observable(routeTypeText[itineraryModel.Type]);
        }

    });

    return {
        // Public:
        routeType: type,



    };
};