
// scheduleTypePageID - The element ID of the page
SmartRoutes.ScheduleTypeFormPageController = (function(pageID) {

    // Private: 
    var scheduleTypeFormPageID = pageID;
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
        },

        StopPage: function() {
            pageValidationCallback = null;
        },

        IsPageDataValid: function() {
            // A schedule type must be selected.
            return !scheduleTypeViewModel.noScheduleTypeSelected();
        },

        GetFormPageID: function() {
            return scheduleTypeFormPageID;
        },

        GetScheduleTypeInformationPayload: function() {
            var payload = new SmartRoutes.Communication.ScheduleTypePayload(scheduleTypeViewModel.dropOffChecked(), scheduleTypeViewModel.pickUpChecked());

            return payload;
        }
    };
});
