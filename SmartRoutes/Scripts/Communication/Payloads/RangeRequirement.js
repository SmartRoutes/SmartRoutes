
SmartRoutes.Communication.RangeRequirement = function(propertyName, propertyMinValue, propertyMaxValue) {

    var name = propertyName;
    var min = propertyMinValue;
    var max = propertyMaxValue;
    var type = "RangeRequirement";

    return {
        GetTransactionObject: function() {
            return {
                PropertyName: name,
                MinValue: min,
                MaxValue: max,
                Type: type
            };
        }
    }
};
