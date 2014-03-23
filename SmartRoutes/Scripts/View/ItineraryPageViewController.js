
SmartRoutes.ItineraryPageViewController = function(pageID) {
    // Private:

    var itineraryPageViewID = pageID;
    var results = null;
    var resultIndex = 0;
    var children = null;
    var childCareDescriptionViewModels = new Array();

    var elementIDs = {
        itineraryDropOffList: "sr-itinerary-drop-off-list",
        itineraryPickUpList: "sr-itinerary-pick-up-list",
        itineraryDropOffContainer: "sr-itinerary-drop-off-container",
        itineraryPickUpContainer: "sr-itinerary-pick-up-container",
        childCareServiceDescriptionTemplate: "sr-child-care-service-description-template",
        childCareServiceDescriptionContainer: "sr-child-care-service-description-container",
    };

    var actions = {
        Depart: {
            type: 0,
            templateID: "sr-itinerary-step-depart",
            template: null
        },

        Arrive: {
            type: 1,
            templateID:"sr-itinerary-step-arrive",
            template: null
        },
        
        DropOff: {
            type: 2,
            templateID: "sr-itinerary-step-drop-off",
            template: null
        },

        PickUp: {
            type: 3,
            templateID: "sr-itinerary-step-pick-up",
            template: null
        },

        Board: {
            type: 4,
            templateID: "sr-itinerary-step-board",
            template: null
        },

        Exit: {
            type: 5,
            templateID: "sr-itinerary-step-exit",
            template: null
        }
    };

    function CompileTemplates() {
        $.each(actions, function(actionName, action) {
            var templateSource = $("#" + action.templateID).html();
            action.template = Handlebars.compile(templateSource);
        });
    };

    (function Init() {
        // Grab all the templates.
        CompileTemplates();
    })();

    function AppendItinerarySteps(node, itinerary) {
        $.each(itinerary.ItineraryActions, function(index, action) {
            var template = null;
            var templateData = null;

            if (action.Action == actions.Arrive.type) {
                templateData = {
                    address: action.Address
                };
                template = actions.Arrive.template;
            }
            else if (action.Action == actions.Depart.type) {
                templateData = {
                    address: action.Address
                };
                template = actions.Depart.template;
            }
            else if (action.Action == actions.DropOff.type) {
                var childrenForThisAction = new Array();

                $.each(action.ChildIndices, function(index, childIndex) {
                    childrenForThisAction.push(children[childIndex]);
                });

                templateData = {
                    children: childrenForThisAction,
                    serviceName: action.ServiceName
                };
                template = actions.DropOff.template;
            }
            else if (action.Action == actions.PickUp.type) {
                var childrenForThisAction = new Array();

                $.each(action.ChildIndices, function(index, childIndex) {
                    childrenForThisAction.push(children[childIndex]);
                });

                templateData = {
                    children: childrenForThisAction,
                    serviceName: action.ServiceName
                }
                template = actions.PickUp.template;
            }
            else if (action.Action == actions.Board.type) {
                templateData = {
                    routeNumber: action.RouteNumber,
                    time: action.Time,
                    stopName: action.StopName
                };
                template = actions.Board.template;
            }
            else if (action.Action == actions.Exit.type) {
                templateData = {
                    time: action.Time,
                    stopName: action.StopName
                };
                template = actions.Exit.template;
            }

            node.append(template(templateData));
        });
    };

    function PopulateItineraryList(routeModel) {
        // Handle the drop off itinerary (if it exists).
        if (routeModel.DropOffPlan && routeModel.DropOffPlan.ItineraryActions) {
            var dropOffList = $("#" + elementIDs.itineraryDropOffList);
            dropOffList.empty();
            AppendItinerarySteps(dropOffList, routeModel.DropOffPlan);
            $("#" + elementIDs.itineraryDropOffContainer).show();
        }
        else {
            $("#" + elementIDs.itineraryDropOffContainer).hide();
        }

        // Handle the pick up itinerary (if it exists).
        if (routeModel.PickUpPlan && routeModel.PickUpPlan.ItineraryActions) {
            var pickUpList = $("#" + elementIDs.itineraryPickUpList);
            pickUpList.empty();
            AppendItinerarySteps(pickUpList, routeModel.PickUpPlan);
            $("#" + elementIDs.itineraryPickUpContainer).show();
        }
        else {
            $("#" + elementIDs.itineraryPickUpContainer).hide();
        }
    };

    function PopulateChildCareInfo(allChildCares, indicesToDisplay) {
        if (allChildCares && indicesToDisplay) {
            var descriptionHTML = $("#" + elementIDs.childCareServiceDescriptionTemplate).html();
            var descriptionContainer = $("#" + elementIDs.childCareServiceDescriptionContainer);

            // Remove any existing bindings.
            ko.cleanNode(descriptionContainer);

            // Clear out the view models.
            childCareDescriptionViewModels = new Array();

            // Remove any existing nodes.
            descriptionContainer.empty();

            // Insert the new nodes and bind them.
            $.each(indicesToDisplay, function(arrayIndex, childCareIndex) {
                if ((childCareIndex >= 0) && (childCareIndex < allChildCares.length)) {
                    descriptionContainer.append(descriptionHTML);

                    // Add a viewmodel for the child care.
                    var viewModel = new SmartRoutes.ChildCareDescriptionViewModel(allChildCares[childCareIndex]);
                    childCareDescriptionViewModels.push(viewModel);

                    // Get the element we just added.
                    var addedElement = descriptionContainer.children().last();
                    
                    // Bind the view model to the element.
                    if (addedElement.length > 0) {
                        ko.applyBindings(viewModel, addedElement[0]);
                    }
                }
            });
        }
    };

    function ShowItinerary(index) {
        // 1) Get the route
        // 2) Populate the itinerary list
        // 3) Populate the child care summaries
        if (results) {
            if (results.Routes && (results.Routes.length > 0)
                && (index < results.Routes.length) && (index >= 0)) {
                var routeModel = results.Routes[index];
                PopulateItineraryList(routeModel);

                if (results.ChildCares) {
                    PopulateChildCareInfo(results.ChildCares, results.Routes[index].ChildCareIndices);
                }
            }
        }
    };

    return {
        // Public:

        RunPage: function(searchResults, childNames) {
            results = searchResults;
            children = childNames;
            var indexFinder = new RegExp("index=([0-9]*)");
            var indexResults = indexFinder.exec(window.location.toString());
            if (indexResults.length > 1) {
                // Just assume it's the second parameter for now.
                resultIndex = parseInt(indexResults[1]);
            }
            ShowItinerary(resultIndex);
        },

        StopPage: function() {

        },

        GetPageViewID: function() {
            return itineraryPageViewID;
        }
    };
};