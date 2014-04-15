
SmartRoutes.PickUpDepartureViewModel = function() {

    return {
        pickUpDepartureTime: ko.observable("17:00"),
        pickUpDepartureSameAsDestination: ko.observable(true),
        pickUpDepartureAddressViewModel: new SmartRoutes.AddressViewModel()
    };
};