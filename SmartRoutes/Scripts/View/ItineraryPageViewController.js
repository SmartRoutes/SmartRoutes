
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
        itineraryDropOffMap: "sr-itinerary-drop-off-map",
        itineraryPickUpMap: "sr-itinerary-pick-up-map",
        childCareServiceDescriptionTemplate: "sr-child-care-service-description-template",
        childCareServiceDescriptionContainer: "sr-child-care-service-description-container",
    };
    
    var addressIconPath = "Content/Images/AddressIcon24.png";
    var serviceIconPath = "Content/Images/ServiceIcon24.png";
    var routeIconPath = "Content/Images/RouteIcon24.png";

    var dropOffMap = null;
    var pickUpMap = null;

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

    // Functions for showing the itinerary steps
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
                PopulateMaps(routeModel);

                if (results.ChildCares) {
                    PopulateChildCareInfo(results.ChildCares, results.Routes[index].ChildCareIndices);
                }
            }
        }
    };
    
    // Functions for showing the maps

    // get the latitude and longitude from an ILocation
    function GetLocation(obj) {
        return [obj.Latitude, obj.Longitude];
    }

    function GetActionMarker(action) {
        var iconUrl = "";
        switch (action.Action) {
            case actions.Depart.type:
            case actions.Arrive.type:
                iconUrl = addressIconPath;
                break;
            case actions.DropOff.type:
            case actions.PickUp.type:
                iconUrl = serviceIconPath;
                break;
            case actions.Board.type:
            case actions.Exit.type:
                iconUrl = routeIconPath;
                break;
        }

        return L.marker(GetLocation(action), {
            icon: L.icon({
                iconUrl: iconUrl,
                iconSize: L.point(24, 24)
            }),
        });
    }

    function GetActionPopupContent(action) {
        switch (action.Action) {
            case actions.Depart.type:
            case actions.Arrive.type:
                return action.Address;
            case actions.DropOff.type:
            case actions.PickUp.type:
                return action.ServiceName;
            case actions.Board.type:
            case actions.Exit.type:
                return action.RouteNumber;
        }
        return "";
    }

    function GetActionPolyline(previousAction, currentAction) {
        var path;
        if (currentAction.Action == 5) {
            // create the bus path
            path = [];
            for (var j = 0; j < currentAction.RoutePath.length; j++) {
                path.push(GetLocation(currentAction.RoutePath[j]));
            };

            // create the polyline
            return new L.polyline(path, {
                color: '#f00'
            });
        } else {
            // create the walking path
            path = [
                GetLocation(previousAction),
                GetLocation(currentAction),
            ]; // create the polyline
            return new L.polyline(path, {
                dashArray: '4,6',
                weight: 3,
                color: '#00f'
            });
        }
    }

    function InitializeMap(parentId, itineraryActions) {
        // get the element
        var mapElement = $("#" + parentId + " .sr-map-canvas")[0];
        
        // create the base map
        var map = L.map(mapElement);
        var osm = L.tileLayer('http://otile1.mqcdn.com/tiles/1.0.0/map/{z}/{x}/{y}.png', {
            attribution: 'Map data &copy; <a href="http://mapquest.com">MapQuest</a>'
        });
        map.addLayer(osm);

        console.log(itineraryActions);
        
        // generate the markers and polylines
        var markers = [];
        var polylines = [];
        var allPoints = [];
        var i;
        for (i = 0; i < itineraryActions.length; i++) {
            var previousAction = itineraryActions[i - 1];
            var currentAction = itineraryActions[i];

            // keep track of all itinery locations
            allPoints.push(GetLocation(currentAction));

            // generate the marker for this action
            var marker = GetActionMarker(currentAction);
            marker.bindPopup(GetActionPopupContent(currentAction));
            markers.push(marker);

            // generate the polyline for this action
            if (i > 0) {
                polylines.push(GetActionPolyline(previousAction, currentAction));
            }
        }

        // add the polylines
        for (i = 0; i < polylines.length; i++) {
            polylines[i].addTo(map);
        };

        // add the markers
        for (i = markers.length - 1; i >= 0; i--) {
            markers[i].addTo(map);
        };

        // zoom to show the entire route
        map.fitBounds(L.polyline(allPoints).getBounds());

        return map;
    }
    
    function PopulateMaps(routeModel) {
        // Handle the drop off itinerary (if it exists).
        if (routeModel.DropOffPlan && routeModel.DropOffPlan.ItineraryActions) {
            $("#" + elementIDs.itineraryDropOffMap).show();
            if (dropOffMap) {
                dropOffMap.remove();
            }
            dropOffMap = InitializeMap(elementIDs.itineraryDropOffMap, routeModel.DropOffPlan.ItineraryActions);
        } else {
            $("#" + elementIDs.itineraryDropOffMap).hide();
        }

        // Handle the pick up itinerary (if it exists).
        if (routeModel.PickUpPlan && routeModel.PickUpPlan.ItineraryActions) {
            $("#" + elementIDs.itineraryPickUpMap).show();
            if (pickUpMap) {
                pickUpMap.remove();
            }
            pickUpMap = InitializeMap(elementIDs.itineraryPickUpMap, routeModel.PickUpPlan.ItineraryActions);
        } else {
            $("#" + elementIDs.itineraryPickUpMap).hide();
        }
    }

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