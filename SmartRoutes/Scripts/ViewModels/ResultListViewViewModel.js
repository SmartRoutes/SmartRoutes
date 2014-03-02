
SmartRoutes.ResultListViewViewModel = function() {
    // Private: 

    var listViewElements = ko.observableArray();

    function Reinitialize(results) {
        // Transform the results into a summary for the results page.
        if (searchResults.Routes) {
            $.each(searchResults.Routes, function(key, route) {
                var services = ko.observableArray();

                var isPickUp = ko.observable(false);
                var isDropOff = ko.observable(false);

                var dropOffRoutes = ko.observable("");
                var pickUpRoutes = ko.observable("");

                // Pull information off the indexed child cares
                // and add it to the view model.  There can be multiple
                // child cares.
                $.each(route.ChildCareIndices, function(key, index) {

                    if (index >= 0 && index < searchResults.ChildCares.length) {
                        var childCareModel = searchResults.ChildCares[index];

                        if (childCareModel) {
                            var serviceName = ko.obserable(childCareModel.ChildCareName);
                            var link = ko.observable(childCareModel.Link);

                            services.push({
                                serviceName: serviceName,
                                serviceLink: link
                            });
                        }
                    }
                    else {
                        console.warn("Transformed child care index out of range.")
                    }
                });

                // Gather drop off plan information.
                if (route.DropOffPlan) {
                    listElement.isDropOffPlan = true;

                    dropOffRoutes = route.DropOffPlan.Routes;
                }

                // Gather pick up plan information.
                if (route.PickUpPlan) {
                    listElement.isPickUpPlan = true;

                    pickUpRoutes = route.PickUpPlan.Routes;
                }

                listViewElements.push({
                    childCareServices: services,
                    isDropOffPlan: isDropOff,
                    dropOffRoutes: dropOffRoutes,
                    isPickUpPlan: pickUpRoutes
                });
            });
        }
    }

    return {
        // Public: 
        elements: listViewElements,

        SetNewResults: function(searchResults) {
            Reinitialize(searchResults);
        }
    }
};