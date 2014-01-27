
SmartRoutes.AccreditationFormPageController = function(pageID) {
    // Private:

    var accreditations = new Array();
    var accreditationViewRaw = null;
    var accreditationFormPageID = pageID;
    var validationCallback = null;

    function CreateAndBindAccreditationViews() {
        // This might be called multiple times since we can't 
        // be if the json data or html will be done last.
        if (accreditationViewRaw && (accreditations.length > 0)) {
            var accreditationListContainer = $("#sr-accreditation-list-container");
            for (var accreditationIndex = 0; accreditationIndex < accreditations.length; ++accreditationIndex) {
                accreditationListContainer.append(accreditationViewRaw);
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
        }
    };
};
