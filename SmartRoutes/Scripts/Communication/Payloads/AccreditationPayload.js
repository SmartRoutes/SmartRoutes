
SmartRoutes.Communication.AccreditationPayload = function(accreditationViewModel) {

    return {
        name: accreditationViewModel.name(),
        checked: accreditationViewModel.checked()
    };
};