
SmartRoutes.LocationAndTimeFormPageController = function(pageID) {
    // Private:

    var locationAndTimeViewModel = null;
    var validationCallback = null;
    var scheduleType = null;
    var locationTimeFormPageID = pageID;

    var elementIDs = {
        dropOffDeparture: "sr-section-drop-off-departure",
        dropOffDestination: "sr-section-drop-off-final-destination",
        pickUpDeparture: "sr-section-pick-up-departure",
        pickUpDestination: "sr-section-pick-up-final-destination",
        pickUpDepartureAddressContainer: "sr-pick-up-departure-address-container",
        pickUpDestinationAddressContainer: "sr-pick-up-destination-address-container",
        duplicateDestinationOptionContainer: "sr-duplicate-drop-off-destination-option-container",
        duplicateDepartureOptionContainer: "sr-duplicate-drop-off-departure-option-container",
    };

    var elementClasses = {
        geocodeFailure: "sr-geocode-fail-error",
    };

    function UpdateAddressViewModelsFromUISelection() {
        if (locationAndTimeViewModel.pickUpDepartureViewModel.pickUpDepartureSameAsDestination()) {
            locationAndTimeViewModel.pickUpDepartureViewModel.pickUpDepartureAddressViewModel.CopyFromAddress(
                locationAndTimeViewModel.dropOffDestinationViewModel.dropOffDestinationAddressViewModel);
        }

        if (locationAndTimeViewModel.pickUpDestinationViewModel.dropOffDestinationSameAsDeparture()) {
            locationAndTimeViewModel.pickUpDestinationViewModel.pickUpDestinationAddressViewModel.CopyFromAddress(
                locationAndTimeViewModel.dropOffDepartureViewModel.dropOffDepartureAddressViewModel);
        }
    };

    // Handles clicking the "address same as..." checkboxes by hiding/showing the fields.
    function SameAddressCallback(container, sameAsDropOff, sourceAddress, destinationAddress) {
        if (sameAsDropOff) {
            container.hide();
        }
        else {
            container.show();
        }
    };

    function BindCallbacks() {
        locationAndTimeViewModel.pickUpDepartureViewModel.pickUpDepartureSameAsDestination.subscribe(function(newValue) {
            SameAddressCallback($("#" + elementIDs.pickUpDepartureAddressContainer), newValue);
        });

        locationAndTimeViewModel.pickUpDestinationViewModel.dropOffDestinationSameAsDeparture.subscribe(function (newValue) {
            SameAddressCallback($("#" + elementIDs.pickUpDestinationAddressContainer), newValue);
        });
    };

    // Handles binding the UI elements to javascript objects for data.
    function InitBindings() {
        locationAndTimeViewModel = new SmartRoutes.LocationAndTimeViewModel();

        ko.applyBindings(locationAndTimeViewModel.dropOffDepartureViewModel.dropOffDepartureAddressViewModel,
                        $("#sr-drop-off-departure-address-container")[0]);

        ko.applyBindings(locationAndTimeViewModel.dropOffDestinationViewModel.dropOffDestinationAddressViewModel,
                         $("#sr-drop-off-destination-address-container")[0]);
        ko.applyBindings(locationAndTimeViewModel.dropOffDestinationViewModel,
                        $("#sr-drop-off-final-destination-supplemental-input")[0]);

        ko.applyBindings(locationAndTimeViewModel.pickUpDepartureViewModel,
                         $("#sr-pick-up-departure-supplemental-input")[0]);
        ko.applyBindings(locationAndTimeViewModel.pickUpDepartureViewModel.pickUpDepartureAddressViewModel,
                         $("#sr-pick-up-departure-address-container")[0]);

        ko.applyBindings(locationAndTimeViewModel.pickUpDestinationViewModel,
                         $("#sr-pick-up-destination-supplemental-input")[0]);
        ko.applyBindings(locationAndTimeViewModel.pickUpDestinationViewModel.pickUpDestinationAddressViewModel,
                         $("#sr-pick-up-destination-address-container")[0]);

        BindCallbacks();
    };

    (function Init() {
        InitBindings();

        // Kinda hackish, but forcing these elements to notify subscribers of their initial
        // value so the ui is setup correctly.
        locationAndTimeViewModel.pickUpDepartureViewModel.pickUpDepartureSameAsDestination.valueHasMutated();
        locationAndTimeViewModel.pickUpDestinationViewModel.dropOffDestinationSameAsDeparture.valueHasMutated();
    })();

    // Shows/hides the location and time sections depending on the
    // schedule type selected.
    function SetupViewsForScheduleType() {
        if (scheduleType.DropOffChecked) {
            $("#" + elementIDs.dropOffDeparture).show();
            $("#" + elementIDs.dropOffDestination).show();
            
            $("#" + elementIDs.duplicateDepartureOptionContainer).show();
            $("#" + elementIDs.duplicateDestinationOptionContainer).show();
        }
        else {
            $("#" + elementIDs.dropOffDeparture).hide();
            $("#" + elementIDs.dropOffDestination).hide();

            $("#" + elementIDs.duplicateDepartureOptionContainer).hide();
            $("#" + elementIDs.duplicateDestinationOptionContainer).hide();

            // These can't be true since there isn't a drop off schedule.
            locationAndTimeViewModel.pickUpDepartureViewModel.pickUpDepartureSameAsDestination(false);
            locationAndTimeViewModel.pickUpDestinationViewModel.dropOffDestinationSameAsDeparture(false);
        }

        if (scheduleType.PickUpChecked) {
            $("#" + elementIDs.pickUpDeparture).show();
            $("#" + elementIDs.pickUpDestination).show();
        }
        else {
            $("#" + elementIDs.pickUpDeparture).hide();
            $("#" + elementIDs.pickUpDestination).hide();
        }
    };

    function SetGeocodeFailureError(section) {
        if (section) {
            $("." + elementClasses.geocodeFailure, section).show();
        }
    };

    return {
        // Public: 

        // Signals that this is now the active form page.
        RunPage: function(pageValidationCallback, scheduleTypeSelection) {
            validationCallback = pageValidationCallback;
            scheduleType = scheduleTypeSelection;

            SetupViewsForScheduleType();
        },

        // Signals that this is no longer the active form page.
        StopPage: function() {
            validationCallback = null;

            UpdateAddressViewModelsFromUISelection();
        },

        // Gets the ID of the form page element.
        GetFormPageID: function() {
            return locationTimeFormPageID;
        },

        SetErrorFromSearchStatus: function(status, statusCodes) {
            var section = null;
            switch (status.Code) {
                case statusCodes.DropOffDepartureGeocodeFail:
                    section = $("#" + elementIDs.dropOffDeparture);
                    break;
                case statusCodes.DropOffDestinationGeocodeFail:
                    section = $("#" + elementIDs.dropOffDestination);
                    break;
                case statusCodes.PickUpDepartureGeocodeFail:
                    section = $("#" + elementIDs.pickUpDeparture);
                    break;
                case statusCodes.PickUpDestinationGeocodeFail:
                    section = $("#" + elementIDs.pickUpDestination);
                    break;
                default:
                    alert("An unexpected error occured.")
                    break;
            }
            SetGeocodeFailureError(section);
        },

        // Gets a data payload object for the form page.
        getLocationAndTimePayload: function() {
            var payload = new SmartRoutes.Communication.LocationsAndTimesPayload();
            var dropOffDepartureViewModel = locationAndTimeViewModel.dropOffDepartureViewModel.dropOffDepartureAddressViewModel;
            payload.DropOffDepartureAddress = new SmartRoutes.Communication.AddressPayload(
                                                dropOffDepartureViewModel.address(),
                                                dropOffDepartureViewModel.addressLine2(),
                                                dropOffDepartureViewModel.city(),
                                                dropOffDepartureViewModel.state(),
                                                dropOffDepartureViewModel.zipCode());

            var dropOffDestinationViewModel = locationAndTimeViewModel.dropOffDestinationViewModel;
            payload.DropOffLatestArrivalTime = dropOffDestinationViewModel.dropOffDestinationLatestTime();

            var dropOffDestinationAddress = dropOffDestinationViewModel.dropOffDestinationAddressViewModel;
            payload.DropOffDestinationAddress = new SmartRoutes.Communication.AddressPayload(
                                                dropOffDestinationAddress.address(),
                                                dropOffDestinationAddress.addressLine2(),
                                                dropOffDestinationAddress.city(),
                                                dropOffDestinationAddress.state(),
                                                dropOffDestinationAddress.zipCode());

            var pickUpDepartureViewModel = locationAndTimeViewModel.pickUpDepartureViewModel;
            payload.PickUpDepartureTime = pickUpDepartureViewModel.pickUpDepartureTime();
            payload.PickUpDepartureAddressSameAsDropOffDestination = pickUpDepartureViewModel.pickUpDepartureSameAsDestination();
            var pickUpDepartureAddressViewModel = pickUpDepartureViewModel.pickUpDepartureAddressViewModel;
            payload.PickUpDepartureAddress = new SmartRoutes.Communication.AddressPayload(
                                                pickUpDepartureAddressViewModel.address(),
                                                pickUpDepartureAddressViewModel.addressLine2(),
                                                pickUpDepartureAddressViewModel.city(),
                                                pickUpDepartureAddressViewModel.state(),
                                                pickUpDepartureAddressViewModel.zipCode());


            var pickUpDestinationViewModel = locationAndTimeViewModel.pickUpDestinationViewModel;
            payload.PickUpDestinationSameAsDropOffDeparture = pickUpDestinationViewModel.dropOffDestinationSameAsDeparture();
            var pickUpDestinationAddress = pickUpDestinationViewModel.pickUpDestinationAddressViewModel;
            payload.PickUpDestinationAddress = new SmartRoutes.Communication.AddressPayload(
                                                pickUpDestinationAddress.address(),
                                                pickUpDestinationAddress.addressLine2(),
                                                pickUpDestinationAddress.city(),
                                                pickUpDestinationAddress.state(),
                                                pickUpDestinationAddress.zipCode());

            return payload;
        }
    };
};