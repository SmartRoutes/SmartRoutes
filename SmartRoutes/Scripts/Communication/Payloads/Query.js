
SmartRoutes.Communication.Query = function() {

    var objectives = new Array();

    return {

        GetTransactionObject: function() {
            var transactionObjectives = new Array();

            $.each(objectives, function(key, value) {
                transactionObjectives.push(value.GetTransactionObject());
            });

            return {
                Objectives: transactionObjectives
            }
        }
    };
};