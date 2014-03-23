
SmartRoutes.FormValidator = function() {

    var fields = ko.observableArray();

    var formValid = ko.computed(function() {
        var valid = true;
        ko.utils.arrayForEach(fields(), function(validationInfo) {
            valid = valid && validationInfo.valid();
        });
        return valid;
    });

    var fieldTypes = {
        text: 0,
    };

    function ValidateTextField(newValue, regexString) {
        var regex = new RegExp(regexString);
        return regex.test(newValue);
    };

    function ValidateTextFieldCallback(newValue) {
        var valid = ValidateTextField(newValue, this.regex);

        // Update the observable field.
        this.valid(valid);

        if (this.callback && (typeof this.callback === "function")) {
            this.callback(valid, this.element);
        }
    };

    function ForceValidateAllFields() {
        $.each(fields(), function(index, info) {
            info.dataBinding.valueHasMutated();
        });
    };

    return {
        // Public:

        // element - jquery object for the dom element
        // validationCallback - function taking a boolean indicating true if the field
        //                      validated successfully, false otherwise
        // section - an integer identifying the section of the form for this field
        AddTextField: function(dataBinding, validationCallback, regex, element) {
            if (dataBinding && regex) {
                var validationInfo = {
                    fieldType: fieldTypes.text,
                    dataBinding: dataBinding,
                    callback: validationCallback,
                    regex: regex,
                    element: element,
                    valid: ko.observable(false),
                };

                fields.push(validationInfo);

                dataBinding.subscribe(ValidateTextFieldCallback, validationInfo);
            }
        },

        IsFormValid: function() {
            return formValid();
        },

        ClearValidators: function() {
            fields([]);
        },

        ValidateAllFields: function() {
            ForceValidateAllFields();
        },
    };
};