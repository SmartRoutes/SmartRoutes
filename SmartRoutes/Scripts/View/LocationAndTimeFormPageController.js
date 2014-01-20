
SmartRoutes.LocationAndTimeFormPageController = function(formPageRouteMap) {
    // Private:

    var pageIDRouteMap = formPageRouteMap;
    var locationAndTimeViewModel = null;

    (function Init() {
        locationAndTimeViewModel = new SmartRoutes.LocationAndTimeViewModel();

        ko.applyBindings(locationAndTimeViewModel, $("#sr-location-time-form-page-view"));

        ko.applyBindings(locationAndTimeViewModel.pickUpDepartAddress, $("#sr-section-drop-off-depart"));
        ko.applyBindings(locationAndTimeViewModel.pickupDestinationAddress, $("#sr-section-drop-off-final-destination"));
        ko.applyBindings(locationAndTimeViewModel.dropOffDepartAddress, $("#sr-section-pick-up-depart"));
        ko.applyBindings(locationAndTimeViewModel.dropOffDestinationAddress, $("#sr-section-pick-up-final-destination"));
    })();

    return {
        // Public: 

        GetLocationAndTimeViewModel: function() {
            return locationAndTimeViewModel;
        }
    };
};