
// Currently unused, but don't want to delete it since
// it might be useful later.

//SmartRoutes.ComboBoxViewModel = function(data) {
//    // Private:

//    var comboboxChoices = ko.observableArray();
//    var comboboxSelection = ko.observable();

//    (function Init() {
//        if (data && Array.isArray(data)) {
//            $.each(data, function(key, value) {
//                comboboxChoices.push(value);
//            });
//        }
//    })();

//    return {
//        // Public:

//        selectedItem: comboboxSelection,
//        choices: comboboxChoices
//    };
//};