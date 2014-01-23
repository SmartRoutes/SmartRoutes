
SmartRoutes.Communication.Requirement = function(propertyName, propertyValue) {
    
    var name = propertyName;
    var type = "Requirement";
    var value = propertyValue;

    return {
        GetTransactionObject: function() {
            return {
                PropertyName: name,
                Type: type,
                Value: value
            }
        }
    };
};