
SmartRoutes.AddressViewModel = function() {
    // Private:

    var address = ko.observable("");
    var addressLine2 = ko.observable("");
    var city = ko.observable("");
    var state = ko.observable("OH");
    var zipCode = ko.observable("");


    return {
        // Public:

        address: address, 
        addressLine2: addressLine2,
        city: city,
        state: state,
        zipCode: zipCode,


        CopyFromAddress: function(other) {
            address(other.address());
            addressLine2(other.addressLine2());
            city(other.city());
            state(other.state());
            zipCode(other.zipCode());
        },

        Clear: function() {
            address = "",
            addressLine2 = "",
            city = "",
            zipCode = ""
        }
    };
};