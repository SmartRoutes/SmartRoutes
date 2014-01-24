
// pageID - The element ID of the page
// validationCallback - Function to be called after the form validates with a value
// indicating if the form is valid (true) or invalid (false).
SmartRoutes.ScheduleTypeFormPageController = (function(pageID) {

    // Private: 
    var scheduleTypePageID = pageID;
    var scheduleTypeViewModel = null;
    var pageValidationCallback = null;

    function ValidateScheduleTypeCallback(noSelectionNewValue) {
        if (pageValidationCallback) {
            var valid = !noSelectionNewValue;
            pageValidationCallback(valid);
        }
    };

    (function Init() {
        scheduleTypeViewModel = new SmartRoutes.ScheduleTypeViewModel();
        scheduleTypeViewModel.noScheduleTypeSelected.subscribe(ValidateScheduleTypeCallback);

        ko.applyBindings(scheduleTypeViewModel, $("#sr-schedule-type-form-page-view")[0]);

    })();

    return {
        // Public:

        RunPage: function(validationCallback) {
            pageValidationCallback = validationCallback;
            $("#" + scheduleTypePageID).fadeIn(SmartRoutes.Constants.formPageFadeInTime);
        },

        StopPage: function() {
            pageValidationCallback = null;
        },

        IsPageDataValid: function() {
            // A schedule type must be selected.
            return !scheduleTypeViewModel.noScheduleTypeSelected();
        },

        GetScheduleTypeInformation: function() {
            var scheduleType = {
                dropOffChecked: scheduleTypeViewModel.dropOffChecked(),
                pickUpChecked: scheduleTypeViewModel.pickUpChecked()
            }

            return scheduleType;
        }
    };

});
