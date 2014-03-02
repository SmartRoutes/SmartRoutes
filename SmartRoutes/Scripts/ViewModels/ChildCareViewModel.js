
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
            var day = weekDayMap[0];
            if (value.Day > 0 && value.Day < weekDayMap.length) {
                day = weekDayMap[value.Day];
            }

            var time = "";
            if (value.OpeningTime && value.ClosingTime) {
                time += value.OpeningTime;
                time += " - ";
                time += value.ClosingTime;
            }

            convertedHours.push({
                day: day,
                timeRange: time
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