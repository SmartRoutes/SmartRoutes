
SmartRoutes.PickUpDestinationViewModel = function() {

    return {

        dropOffDestinationSameAsDeparture: ko.observable(true),
        pickUpDestinationAddressViewModel: new SmartRoutes.AddressViewModel()
    };
};