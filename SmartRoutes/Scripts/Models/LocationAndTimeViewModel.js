
SmartRoutes.LocationAndTimeViewModel = function() {
    // Private: 

    return {
        // Public:

        // Pick up schedule departure information.
        pickUpDepartAddress: new SmartRoutes.AddressViewModel(),
        

        // Pick up schedule destination information.
        pickupDestinationAddress: new SmartRoutes.AddressViewModel(),
        dropOffLatestArrivalTime: ko.observable(""),

        // Drop off schedule departure information.
        pickUpDepartureTime: ko.observable(""),
        pickUpDepartureSameAsDestination: ko.observable(true),
        dropOffDepartAddress: new SmartRoutes.AddressViewModel(),

        // Drop off schedule destination information.
        dropOffDestinationAddress: new SmartRoutes.AddressViewModel(),
        dropOffDestinationSameAsDeparture: ko.observable(true)
    };
};