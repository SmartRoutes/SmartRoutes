
// This is the payload object that 
SmartRoutes.Communication.ServiceTypePayload = function(serviceTypeViewModel) {

    return {
        // The name for the service type.
        Name: serviceTypeViewModel.name(),
        // True if the service type is checked, false otherwise.
        Checked: serviceTypeViewModel.checked()
    }
};