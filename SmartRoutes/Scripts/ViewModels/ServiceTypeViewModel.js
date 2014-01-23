
SmartRoutes.ServiceTypeViewModel = function(serviceTypeName, serviceTypeDescription, serviceTypeChecked) {
    // Private:

    return {
        // Public:
        name: ko.observable(serviceTypeName),
        description: ko.observable(serviceTypeDescription),
        checked: ko.observable(serviceTypeChecked)
    }
};