﻿
// This controller manages the service type (i.e. home type) form page.
SmartRoutes.ServiceTypeFormPageController = function(pageID) {
    // Private:
    
    var serviceTypeFormPageID = pageID;
    var serviceTypes = new Array();
    var serviceTypeViewRaw = null;
    var validationCallback = null;
    var detailedCheckboxViewControllers = new Array();

    // Adds service type views to the page and binds them to data objects.
    function CreateAndBindServiceTypeViews() {
        if (serviceTypeViewRaw && (serviceTypes.length > 0)) {
            var serviceTypeListContainer = $("#sr-service-type-list-container");
            for (var serviceTypeIndex = 0; serviceTypeIndex < serviceTypes.length; ++serviceTypeIndex) {
                serviceTypeListContainer.append(serviceTypeViewRaw);

                var detailedCheckboxView = serviceTypeListContainer.children().last().children(".sr-detailed-checkbox-view");
                var detailedCheckboxController = new SmartRoutes.DetailedCheckboxViewController(detailedCheckboxView, null);
                detailedCheckboxViewControllers.push(detailedCheckboxController);

                ko.applyBindings(serviceTypes[serviceTypeIndex], detailedCheckboxView[0]);
            }
        }
    };

    (function Init() {
        var guidedSearchCommunicationController = new SmartRoutes.Communication.GuidedSearchCommunicationController();

        // We need to get information for the service types.
        guidedSearchCommunicationController.FetchServiceTypes(function(serviceTypeData) {
            $.each(serviceTypeData, function(key, value) {
                serviceTypes.push(new SmartRoutes.ServiceTypeViewModel(value.Name, value.Description, value.Checked));
            });
            CreateAndBindServiceTypeViews();
        });

        // We need to fetch the HTML for the service type view.
        guidedSearchCommunicationController.FetchServiceTypeView(function(serviceTypeView) {
            serviceTypeViewRaw = serviceTypeView;
            CreateAndBindServiceTypeViews();
        });
    })();

    return {
        // Public: 

        // Signals that the form page is now the active page.
        RunPage: function(pageValidationCallback) {
            validationCallback = pageValidationCallback;
        },

        // Signals that this form page is no longer the active page.
        StopPage: function() {
            validationCallback = null;
        },

        // Gets the ID for the form page elements.
        GetFormPageID: function() {
            return serviceTypeFormPageID;
        },

        IsPageValid: function() {
            // TODO: 
            return true;
        },
        
        // Gets an array of service type payload objects based on
        // the current state of the service type form.
        GetServiceTypePayloadArray: function() {
            var serviceTypePayloadArray = new Array();
            $.each(serviceTypes, function(key, value) {
                serviceTypePayloadArray.push(new SmartRoutes.Communication.ServiceTypePayload(value));
            });

            return serviceTypePayloadArray;
        }
    };
};