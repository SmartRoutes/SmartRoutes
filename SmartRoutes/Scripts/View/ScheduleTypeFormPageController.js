
SmartRoutes.ScheduleTypeFormPageController = (function(pageID) {

    // Private: 
    var scheduleTypePageID = pageID;
    var scheduleTypeViewModel = null;

    ko.extenders.scheduleTypeRequired = function(target, message) {


        return target;
    };

    (function Init() {
        scheduleTypeViewModel = new SmartRoutes.ScheduleTypeViewModel();
        ko.applyBindings(scheduleTypeViewModel, $("#sr-schedule-type-form-page-view")[0]);
    })();

    return {
        // Public:

        RunPage: function() {

            $("#" + scheduleTypePageID).fadeIn(SmartRoutes.Constants.formPageFadeInTime);
        },

        GetScheduleTypeViewModel: function() {
            var scheduleType = {
                dropOffChecked: scheduleTypeViewModel.dropOffChecked(),
                pickUpChecked: scheduleTypeViewModel.pickUpChecked()
            }

            return scheduleType;
        }
    };

});
