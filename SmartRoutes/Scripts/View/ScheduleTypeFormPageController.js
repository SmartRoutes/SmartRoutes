
SmartRoutes.ScheduleTypeFormPageController = (function(pageID) {

    // Private: 
    var scheduleTypePageID = pageID;
    var scheduleTypeViewModel = null;

    (function Init() {
        scheduleTypeViewModel = new SmartRoutes.ScheduleTypeViewModel();
        ko.applyBindings(scheduleTypeViewModel, $("#sr-schedule-type-input")[0]);
    })();

    return {
        // Public:

        RunPage: function() {

            $("#" + scheduleTypePageID).fadeIn(SmartRoutes.Constants.formPageFadeInTime);
        },

        GetScheduleTypeViewModel: function() {
            return scheduleTypeViewModel;
        }
    };

});
