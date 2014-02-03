
SmartRoutes.Communication.Objective = function() {

    var requirements = new Array();
    

    return {

        GetTransactionObject: function() {
            var requirementsTemp = new Array();

            $.each(requirements, function(key, value) {
                requirementsTemp.push(value.GetTransactionObject());
            });

            return {
                Requirements: requirementsTemp
            }
        }
    };
};
