
SmartRoutes.LocationAndTimeViewModel = function() {
    // Private: 

    //var GetCurrentTimeString = function() {
    //    var currentTime = new Date();
    //    var timeString = currentTime.getHours() + ":" + currentTime.getMinutes()
    //                     + ":" + currentTime.getSeconds();
    //    return timeString;
    //};

    return {
        // Public:

        // Pick up schedule departure information.
        dropOffDepartureViewModel: new SmartRoutes.DropOffDepartureViewModel(),

        dropOffDestinationViewModel: new SmartRoutes.DropOffDestinationViewModel(),

        pickUpDepartureViewModel: new SmartRoutes.PickUpDepartureViewModel(),

        pickUpDestinationViewModel: new SmartRoutes.PickUpDestinationViewModel()
    };
};