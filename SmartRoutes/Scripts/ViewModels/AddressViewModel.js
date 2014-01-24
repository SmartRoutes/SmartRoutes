
SmartRoutes.AddressViewModel = function() {
    // Private:

    return {
        // Public:

        address: ko.observable(""),
        addressLine2: ko.observable(""),
        city: ko.observable(""),
        state: ko.observable("OH"),
        zipCode: ko.observable("")
    };
};