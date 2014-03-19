
SmartRoutes.ResultListViewViewModel = function() {
    // Private: 

    var listViewElements = ko.observableArray();

    function Reinitialize(searchResults) {
        // Transform the results into a summary for the results page.
        if (searchResults.Routes) {
            // Clear the list.
            listViewElements([]);

            $.each(searchResults.Routes, function(key, route) {
                var services = new Array();

                var isPickUp = false;
                var isDropOff = false;

                var dropOffRoutes = "";
                var pickUpRoutes = "";

                var navLink = "#/itinerary?index=" + key;

                // Pull information off the indexed child cares
                // and add it to the view model.  There can be multiple
                // child cares.
                $.each(route.ChildCareIndices, function(key, index) {

                    if (index >= 0 && index < searchResults.ChildCares.length) {
                        var childCareModel = searchResults.ChildCares[index];

                        if (childCareModel) {
                            var serviceName = childCareModel.ChildCareName;
                            var link = childCareModel.Link;

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
                    isDropOff = true;

                    dropOffRoutes = route.DropOffPlan.Routes;
                }

                // Gather pick up plan information.
                if (route.PickUpPlan) {
                    isPickUp = true;

                    pickUpRoutes = route.PickUpPlan.Routes;
                }

                listViewElements.push({
                    childCareServices: services,
                    isDropOffPlan: isDropOff,
                    dropOffRoutes: dropOffRoutes,
                    isPickUpPlan: isPickUp,
                    pickUpRoutes: pickUpRoutes,
                    navLink: navLink
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