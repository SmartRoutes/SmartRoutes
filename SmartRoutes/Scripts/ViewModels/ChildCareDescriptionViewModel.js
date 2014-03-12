
SmartRoutes.ChildCareDescriptionViewModel = function(childCareModel) {
    // Private: 

    var name = ko.observable("");
    var address = ko.observable("");
    var phoneNumber = ko.observable("");
    var link = ko.observable("");
    var reviewLink = ko.observable("");
    var hours = ko.observableArray(null);

    var dayMap = [
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday"
    ];

    function SetupHours() {
        hours.removeAll();
        $.each(childCareModel.Hours, function(index, hourModel) {
            var day = dayMap[0];
            if (hourModel.Day && (hourModel.Day > 0) && (hourModel.Day < dayMap.length)) {
                day = dayMap[hourModel.Day];
            }

            var timeRange = "";
            if (hourModel.OpeningTime) {
                timeRange += hourModel.OpeningTime;
            }

            if (hourModel.ClosingTime) {
                timeRange += " - ";
                timeRange += hourModel.ClosingTime;
            }

            hours.push({
                day: day,
                timeRange: timeRange
            });
        });
    };

    (function Init() {
        if (childCareModel) {
            name = childCareModel.ChildCareName;
            address = childCareModel.Address;
            phoneNumber = childCareModel.PhoneNumber;
            link = childCareModel.Link;
            reviewLink = childCareModel.ReviewLink;
            SetupHours();
        }
    })();

    return {
        // Public: 
        childCareName: name,
        address: address,
        phoneNumber: phoneNumber,
        link: link,
        reviewLink: reviewLink,
        hours: hours
    };
};