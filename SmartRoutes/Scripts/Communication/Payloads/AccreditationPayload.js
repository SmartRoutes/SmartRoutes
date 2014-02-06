
SmartRoutes.Communication.AccreditationPayload = function(accreditationViewModel) {

    return {
        Name: accreditationViewModel.name(),
        Checked: accreditationViewModel.checked()
    };
};