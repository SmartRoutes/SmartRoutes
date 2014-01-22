
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
        $.get("/GuidedSearchPage/ServiceTypeView", function(data) {
            serviceTypeViewRaw = data;
            CreateAndBindServiceTypeViews();
        }, "html");

        $.getJSON("/GuidedSearchPage/ServiceTypes", function(data) {
            $.each(data, function(key, value) {
                serviceTypes.push(new SmartRoutes.ServiceTypeViewModel(value.Name, value.Description, value.Checked));
            });
            CreateAndBindServiceTypeViews();
        });
    })();

    return {
        // Public: 

    };
};