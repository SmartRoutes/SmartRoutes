
SmartRoutes.AccreditationFormPageController = function(pageID) {
    // Private:

    var accreditations = new Array();
    var accreditationViewRaw = null;
    var accreditationFormPageID = pageID;
    var validationCallback = null;
    var detailedCheckboxViewControllers = new Array();

    function CreateAndBindAccreditationViews() {
        // This might be called multiple times since we can't 
        // be if the json data or html will be done last.
        if (accreditationViewRaw && (accreditations.length > 0)) {
            var accreditationListContainer = $("#sr-accreditation-list-container");
            for (var accreditationIndex = 0; accreditationIndex < accreditations.length; ++accreditationIndex) {
                accreditationListContainer.append(accreditationViewRaw);
                var detailedCheckboxView = $(accreditationListContainer).children().last(".sr-accreditation-view").children(".sr-detailed-checkbox-view");

                var checkboxController = new SmartRoutes.DetailedCheckboxViewController(detailedCheckboxView, null);
                checkboxController.SetExtraContent("<a class=\"sr-accreditation-link\" target=\"_blank\" data-bind=\"attr: {href: url}, text: url\"></a>")

                detailedCheckboxViewControllers.push(checkboxController);

                ko.applyBindings(accreditations[accreditationIndex], accreditationListContainer.children().last()[0]);
            }
        }
    };

    (function Init() {
        var guidedSeachCommunicationController = new SmartRoutes.Communication.GuidedSearchCommunicationController();

        guidedSeachCommunicationController.FetchAccreditationView(function(accreditationView) {
            accreditationViewRaw = accreditationView;
            CreateAndBindAccreditationViews();
        });

        guidedSeachCommunicationController.FetchAccreditations(function(accreditationModels) {
            $.each(accreditationModels, function(key, value) {
                accreditations.push(new SmartRoutes.AccreditationViewModel(value.Name, value.Description, value.URL, value.Checked));
            });
            CreateAndBindAccreditationViews();
        });
    })();

    return {
        // Public:

        RunPage: function(pageValidationCallback) {
            validationCallback = pageValidationCallback;
        },

        StopPage: function() {
            validationCallback = null;
        },

        GetFormPageID: function() {
            return accreditationFormPageID;
        },

        GetAccreditationPayloadArray: function() {
            var accreditationPayloadArray = new Array();

            $.each(accreditations, function(key, value) {
                accreditationPayloadArray.push(new SmartRoutes.Communication.AccreditationPayload(value));
            });

            return accreditationPayloadArray;
        }
    };
};
