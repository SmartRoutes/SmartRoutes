﻿
SmartRoutes.DropOffDestinationViewModel = function() {

    return {
        dropOffDestinationAddressViewModel: new SmartRoutes.AddressViewModel(),
        dropOffDestinationLatestTime: ko.observable("")
    }
};