
SmartRoutes.LocationAndTimeFormPageController = function(pageID) {
    // Private:

    var locationAndTimeViewModel = null;
    var validationCallback = null;
    var scheduleType = null;
    var locationTimeFormPageID = pageID;

    var locationTimeSectionIDs = {
        dropOffDeparture: "sr-section-drop-off-departure",
        dropOffDestination: "sr-section-drop-off-final-destination",
        pickUpDeparture: "sr-section-pick-up-departure",
        pickUpDestination: "sr-section-pick-up-final-destination"
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
    };

    // This function shows the various sections determined by the
    // schedule type page.
    function InitViewSectionVisibility() {

    };

    (function Init() {
        InitBindings();
    })();

    // Shows/hides the location and time sections depending on the
    // schedule type selected.
    function SetupViewsForScheduleType() {
        if (scheduleType.DropOffChecked) {
            $("#" + locationTimeSectionIDs.dropOffDeparture).show();
            $("#" + locationTimeSectionIDs.dropOffDestination).show();
        }
        else {
            $("#" + locationTimeSectionIDs.dropOffDeparture).hide();
            $("#" + locationTimeSectionIDs.dropOffDestination).hide();
        }

        if (scheduleType.PickUpChecked) {
            $("#" + locationTimeSectionIDs.pickUpDeparture).show();
            $("#" + locationTimeSectionIDs.pickUpDestination).show();
        }
        else {
            $("#" + locationTimeSectionIDs.pickUpDeparture).hide();
            $("#" + locationTimeSectionIDs.pickUpDestination).hide();
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
        },

        // Gets the ID of the form page element.
        GetFormPageID: function() {
            return locationTimeFormPageID;
        },

        // Gets a data payload object for the form page.
        getLocationAndTimePayload: function() {
            // Important TODO: forgot to add this,
            return locationAndTimeViewModel;
        }
    };
};