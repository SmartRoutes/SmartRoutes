
SmartRoutes.ChildCareViewModel = function(childCareModel) {
    // Private:

    var model = childCareModel;
    var businessHours = null;

    var weekDayMap = [
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday"
    ];

    (function Init() {
        var convertedHours = new Array();

        $.each(childCareModel.Hours, function(key, value) {
            convertedHours.push({
                day: weekDayMap[value.Day],
                openingTime: value.OpeningTime,
                closingTime: value.closingTime
            });
        });

        hours = ko.observableArray(convertedHours);
    })();

    return {
        // Public: 
        childCareName: ko.observable(model.ChildCareName),
        link: ko.observable(model.Link),
        reviewLink: ko.observable(model.ReviewLink),
        address: ko.observable(model.Address),
        phoneNumber: ko.observable(model.PhoneNumber),
        hours: businessHours
    };
};