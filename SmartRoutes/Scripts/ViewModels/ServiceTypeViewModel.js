
// This the the view model object that the data display
// elements of a service type view can be bound to.
SmartRoutes.ServiceTypeViewModel = function(serviceTypeName, serviceTypeDescription, serviceTypeChecked) {
    // Private:

    return {
        // Public:

        // The name of the service type (i.e. home type).
        name: ko.observable(serviceTypeName),

        // A brief description of the service type.
        description: ko.observable(serviceTypeDescription),

        // True if the service type is checked, false otherwise.
        checked: ko.observable(serviceTypeChecked)
    }
};