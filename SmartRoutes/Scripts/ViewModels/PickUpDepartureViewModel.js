
SmartRoutes.PickUpDepartureViewModel = function() {

    return {
        pickUpDepartureTime: ko.observable(""),
        pickUpDepartureSameAsDestination: ko.observable(true),
        pickUpDepartureAddressViewModel: new SmartRoutes.AddressViewModel()
    };
};