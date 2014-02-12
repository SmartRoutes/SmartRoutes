
SmartRoutes.ScheduleTypeFormPageController = (function(pageID) {

    // Private: 
    var scheduleTypeFormPageID = pageID;
    var scheduleTypeViewModel = null;
    var pageValidationCallback = null;

    // Callback for validating the form when the selection changes.
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

        // Signals that the form page is now the active page.
        RunPage: function(validationCallback) {
            pageValidationCallback = validationCallback;
        },

        // Signals that the form page is no longer the active page.
        StopPage: function() {
            pageValidationCallback = null;
        },

        // Reports if the form page fields are valid.
        IsPageDataValid: function() {
            // A schedule type must be selected.
            return !scheduleTypeViewModel.noScheduleTypeSelected();
        },

        // Gets the ID for the form page element.
        GetFormPageID: function() {
            return scheduleTypeFormPageID;
        },

        // Gets the data payload object for the form page fields.
        GetScheduleTypeInformationPayload: function() {
            var payload = new SmartRoutes.Communication.ScheduleTypePayload(scheduleTypeViewModel.dropOffChecked(), scheduleTypeViewModel.pickUpChecked());

            return payload;
        }
    };
});
