
SmartRoutes.ServiceTypeFormPageController = function() {
    // Private:
    
    var serviceTypes = new Array();
    var serviceTypeViewRaw = null;

    function CreateAndBindServiceTypeViews() {
        if (serviceTypeViewRaw && (serviceTypes.length > 0)) {
            var serviceTypeListContainer = $("#sr-service-type-list-container");
            for (var serviceTypeIndex = 0; serviceTypeIndex < serviceTypes.length; ++serviceTypeIndex) {
                serviceTypeListContainer.append(serviceTypeViewRaw);
                ko.applyBindings(serviceTypes[serviceTypeIndex], serviceTypeListContainer.children().last()[0]);
            }
        }
    };

    (function Init() {
        var guidedSearchCommunicationController = new SmartRoutes.Communication.GuidedSearchCommunicationController();

        guidedSearchCommunicationController.FetchServiceTypes(function(serviceTypeData) {
            $.each(serviceTypeData, function(key, value) {
                serviceTypes.push(new SmartRoutes.ServiceTypeViewModel(value.Name, value.Description, value.Checked));
            });
            CreateAndBindServiceTypeViews();
        });

        guidedSearchCommunicationController.FetchServiceTypeView(function(serviceTypeView) {
            serviceTypeViewRaw = serviceTypeView;
            CreateAndBindServiceTypeViews();
        });
    })();

    return {
        // Public: 

    };
};