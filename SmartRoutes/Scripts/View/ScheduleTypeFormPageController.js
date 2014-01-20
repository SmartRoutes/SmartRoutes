
(SmartRoutes.ScheduleTypeFormPageController = function(formPageRouteMap) {

    // Private: 
    var pageIDRouteMap = formPageRouteMap;
    var scheduleTypeViewModel = null;

    (function Init() {
        scheduleTypeViewModel = new SmartRoutes.ScheduleTypeViewModel();
        ko.applyBindings(scheduleTypeViewModel, $("#sr-schedule-type-form-page-view")[0]);
    })();

    return {
        // Public:

        GetScheduleTypeViewModel: function() {
            return scheduleTypeViewModel;
        }
    };

});
