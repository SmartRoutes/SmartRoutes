
SmartRoutes.AccreditationViewModel = function(accreditationName, accreditationDescription, accreditationURL, accreditationChecked) {

    return {
        // Public:

        name: ko.observable(accreditationName),
        description: ko.observable(accreditationDescription),
        url: ko.observable(accreditationURL),
        checked: ko.observable(false)
    };
};