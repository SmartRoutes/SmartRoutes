
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
        if (scheduleType.dropOffChecked) {
            $("#" + locationTimeSectionIDs.dropOffDeparture).show();
            $("#" + locationTimeSectionIDs.dropOffDestination).show();
        }
        else {
            $("#" + locationTimeSectionIDs.dropOffDeparture).hide();
            $("#" + locationTimeSectionIDs.dropOffDestination).hide();
        }

        if (scheduleType.pickUpChecked) {
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

        RunPage: function(pageValidationCallback, scheduleTypeSelection) {
            validationCallback = pageValidationCallback;
            scheduleType = scheduleTypeSelection;

            SetupViewsForScheduleType();

            $("#" + locationTimeFormPageID).fadeIn(SmartRoutes.Constants.formPageFadeInTime);
        },

        StopPage: function() {
            validationCallback = null;
        },

        GetFormPageID: function() {
            return locationTimeFormPageID;
        },

        GetLocationAndTimeViewModel: function() {
            return locationAndTimeViewModel;
        }
    };
};