
SmartRoutes.ScheduleTypeViewModel = (function() {
    // Private:

    var dropOffChecked = ko.observable(true);
    var pickUpChecked = ko.observable(false);
    var noScheduleTypeSelected = ko.computed(function() {
        return !(dropOffChecked() || pickUpChecked());
    });

    return {
        // Public:
        dropOffChecked: dropOffChecked,
        pickUpChecked: pickUpChecked,
        noScheduleTypeSelected: noScheduleTypeSelected
    };
});
