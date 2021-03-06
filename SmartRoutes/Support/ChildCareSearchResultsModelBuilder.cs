﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using PolyGeocoder.Support;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Graph;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Models;
using SmartRoutes.Models.Itinerary;
using SmartRoutes.Models.Payloads;

namespace SmartRoutes.Support
{
    public class ChildCareSearchResultsModelBuilder
    {
        private ChildCareModel[] _childCareModels;
        private ChildCareRoute[] _childCareRoutes;
        private ChildCareRouteModel[] _chldCareRouteModels;
        private Func<IDestination, bool>[] _criteria;
        private IDictionary<IDestination, int> _destinationToIndex;

        private string _dropOffDepartureAddress;
        private ILocation _dropOffDepartureLocation;
        private string _dropOffDestinationAddress;
        private ILocation _dropOffDestinationLocation;
        private SearchResult[] _dropOffSearchResults;

        private IDestination[] _indexToDestination;

        private string _pickUpDepartureAddress;
        private ILocation _pickUpDepartureLocation;
        private string _pickUpDestinationAddress;
        private ILocation _pickUpDestinationLocation;
        private SearchResult[] _pickUpSearchResults;

        private static string GetSearchResultDestinationsKey(IDictionary<IDestination, string> destinationToKey, SearchResult searchResult)
        {
            string destinationsKey = string.Join("-", searchResult
                .Destinations
                .Select(d => destinationToKey[d])
                .OrderBy(k => k));

            return destinationsKey;
        }

        public ChildCareSearchResultsModel Build(ISimpleGeocoder geocoder, ChildCareSearchQueryPayload searchQuery)
        {
            // create the address strings
            _dropOffDepartureAddress = searchQuery.LocationsAndTimes.DropOffDepartureAddress.ToString();
            _dropOffDestinationAddress = searchQuery.LocationsAndTimes.DropOffDestinationAddress.ToString();
            _pickUpDepartureAddress = searchQuery.LocationsAndTimes.PickUpDepartureAddress.ToString();
            _pickUpDestinationAddress = searchQuery.LocationsAndTimes.PickUpDestinationAddress.ToString();

            // create the criteria functions
            _criteria = searchQuery
                .ChildInformation
                .Select(childInformation => CreateCriterion(searchQuery, childInformation))
                .ToArray();

            // search for the drop off plans
            var responses = new Dictionary<string, ILocation>();
            if (searchQuery.ScheduleType.DropOffChecked)
            {
                _dropOffDepartureLocation = geocoder.GetLocationOrNull(responses, _dropOffDepartureAddress).Result;
                if (_dropOffDepartureLocation == null)
                {
                    return new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.DropOffDepartureGeocodeFail)
                    };
                }
                _dropOffDestinationLocation = geocoder.GetLocationOrNull(responses, _dropOffDestinationAddress).Result;
                if (_dropOffDestinationLocation == null)
                {
                    return new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.DropOffDestinationGeocodeFail)
                    };
                }

                _dropOffSearchResults = GraphSingleton.Instance.Graph.Search(
                    _dropOffDestinationLocation,
                    _dropOffDepartureLocation,
                    StandardizeTime(searchQuery.LocationsAndTimes.DropOffLatestArrivalTime),
                    TimeDirection.Backwards,
                    _criteria,
                    10).ToArray();
            }
            else
            {
                _dropOffSearchResults = new SearchResult[0];
            }

            // search for the pick up plans
            if (searchQuery.ScheduleType.PickUpChecked)
            {
                // restrict the destinations explored if we are getting both the drop off and pick up plan
                if (searchQuery.ScheduleType.DropOffChecked)
                {
                    ISet<IDestination> dropOffDestinations = new HashSet<IDestination>(_dropOffSearchResults.SelectMany(s => s.Destinations));
                    _criteria = _criteria
                        .Select(c => RestrictDestinations(c, dropOffDestinations))
                        .ToArray();
                }

                _pickUpDepartureLocation = geocoder.GetLocationOrNull(responses, _pickUpDepartureAddress).Result;
                if (_pickUpDepartureLocation == null)
                {
                    return new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.PickUpDepartureGeocodeFail)
                    };
                }
                _pickUpDestinationLocation = geocoder.GetLocationOrNull(responses, _pickUpDestinationAddress).Result;
                if (_pickUpDestinationLocation == null)
                {
                    return new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.PickUpDestinationGeocodeFail)
                    };
                }

                _pickUpSearchResults = GraphSingleton.Instance.Graph.Search(
                    _pickUpDepartureLocation,
                    _pickUpDestinationLocation,
                    StandardizeTime(searchQuery.LocationsAndTimes.PickUpDepartureTime),
                    TimeDirection.Forwards,
                    _criteria,
                    10).ToArray();
            }
            else
            {
                _pickUpSearchResults = new SearchResult[0];
            }

            // build dependent models
            _indexToDestination = CollectDestinations().ToArray();
            _destinationToIndex = GetDestinationToIndex();
            _childCareModels = GetChildCareModels().ToArray();

            _childCareRoutes = SortChildCareRoutes(GetChildCareRoutes()).ToArray();

            _chldCareRouteModels = GetChildCareRouteModels().ToArray();

            // build the final model
            var model = new ChildCareSearchResultsModel();
            foreach (ChildCareModel childCareModel in _childCareModels)
            {
                model.AddChildCare(childCareModel);
            }
            foreach (ChildCareRouteModel childCareRouteModel in _chldCareRouteModels)
            {
                if ((_dropOffSearchResults.Length == 0) == (childCareRouteModel.DropOffPlan == null) &&
                    (_pickUpSearchResults.Length == 0) == (childCareRouteModel.PickUpPlan == null))
                {
                    model.AddChildCareRoute(childCareRouteModel);
                }
            }
            model.Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.ResultsOk);

            return model;
        }

        private static Func<IDestination, bool> CreateCriterion(ChildCareSearchQueryPayload searchQuery, ChildInformationPayload childInformation)
        {
            return destination =>
            {
                var childCare = destination as ChildCare;
                var detailedChildCare = destination as DetailedChildCare;
                if (childCare == null)
                {
                    return false;
                }

                if (detailedChildCare != null)
                {
                    // check the age group
                    if (ResourceModels.AgeGroupValidators.ContainsKey(childInformation.AgeGroup) && // is the age group valid?
                        ResourceModels.AgeGroupValidators.Values.Any(validate => validate(detailedChildCare)) && // are any age groups reported?
                        !ResourceModels.AgeGroupValidators[childInformation.AgeGroup](detailedChildCare)) // is the age group supported?
                    {
                        return false;
                    }

                    // check the accrediations
                    AccreditationPayload[] checkedAccreditations = searchQuery
                        .Accreditations
                        .Where(a => a.Checked && ResourceModels.AccreditationValidators.ContainsKey(a.Name))
                        .ToArray();
                    if (checkedAccreditations.Any() &&
                        !checkedAccreditations.Any(a => ResourceModels.AccreditationValidators[a.Name](detailedChildCare)))
                    {
                        return false;
                    }
                }

                // check the service type
                ServiceTypePayload[] checkedServiceTypes = searchQuery
                    .ServiceTypes
                    .Where(s => s.Checked && ResourceModels.ServiceTypeValidators.ContainsKey(s.Name))
                    .ToArray();
                if (checkedServiceTypes.Any() &&
                    !checkedServiceTypes.Any(s => ResourceModels.ServiceTypeValidators[s.Name](childCare)))
                {
                    return false;
                }

                return true;
            };
        }

        private static Func<IDestination, bool> RestrictDestinations(Func<IDestination, bool> criteria, ISet<IDestination> destinations)
        {
            return destination => destinations.Contains(destination) && criteria(destination);
        }

        private static DateTime StandardizeTime(DateTime dateTime)
        {
            return new DateTime(
                1970, 1, 1,
                dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }

        private IDictionary<IDestination, int> GetDestinationToIndex()
        {
            return _indexToDestination
                .Select((d, i) => new {Destination = d, Index = i})
                .ToDictionary(d => d.Destination, d => d.Index);
        }

        private IEnumerable<ChildCareRoute> SortChildCareRoutes(IEnumerable<ChildCareRoute> routes)
        {
            return routes
                .OrderBy(r =>
                {
                    TimeSpan dropOffTime = r.DropOffSearchResult != null ? GetTimeSpan(r.DropOffSearchResult.LongResults) : new TimeSpan(0);
                    TimeSpan pickUpTime = r.PickUpSearchResult != null ? GetTimeSpan(r.PickUpSearchResult.LongResults) : new TimeSpan(0);

                    return dropOffTime + pickUpTime;
                });
        }

        private TimeSpan GetTimeSpan(IEnumerable<NodeInfo> nodeInfos)
        {
            return nodeInfos.Last().Node.Time - nodeInfos.First().Node.Time;
        }

        private IEnumerable<ChildCareRouteModel> GetChildCareRouteModels()
        {
            return _childCareRoutes
                .Select((route, i) =>
                {
                    var m = new ChildCareRouteModel();

                    ISet<int> childCareIndices = new HashSet<int>();
                    if (route.DropOffSearchResult != null)
                    {
                        m.DropOffPlan = GetItineraryModel(route.DropOffSearchResult, new DropOffItineraryModel(), childCareIndices);
                    }

                    if (route.PickUpSearchResult != null)
                    {
                        m.PickUpPlan = GetItineraryModel(route.PickUpSearchResult, new PickUpItineraryModel(), childCareIndices);
                    }

                    m.ChildCareIndices = childCareIndices.ToArray();
                    m.ResultPriority = i;

                    return m;
                });
        }

        private IEnumerable<IDestination> CollectDestinations()
        {
            return _dropOffSearchResults
                .SelectMany(sr => sr.Destinations)
                .Concat(_pickUpSearchResults
                    .SelectMany(sr => sr.Destinations))
                .Distinct();
        }

        private T GetItineraryModel<T>(SearchResult searchResult, T model, ISet<int> childCareIndices) where T : ItineraryModel
        {
            string departureAddress;
            ILocation departureLocation;
            string destinationAddress;
            ILocation destinationLocation;
            if (model is DropOffItineraryModel)
            {
                departureAddress = _dropOffDepartureAddress;
                departureLocation = _dropOffDepartureLocation;
                destinationAddress = _dropOffDestinationAddress;
                destinationLocation = _dropOffDestinationLocation;
            }
            else
            {
                departureAddress = _pickUpDepartureAddress;
                departureLocation = _pickUpDepartureLocation;
                destinationAddress = _pickUpDestinationAddress;
                destinationLocation = _pickUpDestinationLocation;
            }

            IList<string> summary = new List<string>();

            IGtfsNode previousGtfs = null;
            model.AddAction(new DepartAction(departureLocation, departureAddress));
            foreach (NodeInfo current in searchResult.ShortResults)
            {
                var currentGtfs = current.Node as IGtfsNode;
                var currentDestination = current.Node as IDestinationNode;
                ChildCare currentChildCare = currentDestination != null ? currentDestination.Destination as ChildCare : null;

                if (currentGtfs != null)
                {
                    string routeNumber = currentGtfs.stopTime.Trip.Headsign ?? currentGtfs.stopTime.Trip.Route.ShortName;
                    if (previousGtfs == null || currentGtfs.TripId != previousGtfs.TripId)
                    {
                        model.AddAction(new BoardBusAction(
                            currentGtfs.stopTime.Stop,
                            routeNumber,
                            current.Node.Time,
                            currentGtfs.stopTime.Stop.Name,
                            currentGtfs.stopTime.Id));

                        summary.Add("RouteNumber:" + currentGtfs.stopTime.Trip.Route.ShortName);
                    }
                    else
                    {
                        model.AddAction(new ExitBusAction(
                            currentGtfs.stopTime.Stop,
                            routeNumber,
                            current.Node.Time,
                            currentGtfs.stopTime.Stop.Name,
                            currentGtfs.stopTime.Id,
                            previousGtfs
                                .stopTime
                                .GetStopsBetween(currentGtfs.stopTime)
                                .Select(s => s.GetLocation())
                                .ToArray()));
                    }

                    previousGtfs = currentGtfs;
                }
                else
                {
                    previousGtfs = null;

                    if (currentChildCare != null)
                    {
                        // infer the children that will be dropped off at this location
                        int[] childIndices = _criteria
                            .Select((c, i) => new {Criterion = c, Index = i})
                            .Where(t => t.Criterion(currentDestination.Destination))
                            .Select(t => t.Index)
                            .ToArray();

                        // get the name of the destination
                        string name = currentDestination.Name;

                        // infer the unique index of this destination
                        childCareIndices.Add(_destinationToIndex[currentDestination.Destination]);

                        if (model is DropOffItineraryModel)
                        {
                            model.AddAction(new DropOffAction(currentChildCare, childIndices, name, currentChildCare.Id));
                        }
                        else
                        {
                            model.AddAction(new PickUpAction(currentChildCare, childIndices, name, currentChildCare.Id));
                        }
                        summary.Add("ServiceName:" + name);
                    }
                }
            }

            model.AddAction(new ArriveAction(destinationLocation, destinationAddress));
            model.Routes = summary;
            model.PathCost = GetTimeSpan(searchResult.LongResults);

            return model;
        }

        private IEnumerable<ChildCareModel> GetChildCareModels()
        {
            foreach (ChildCare cc in _indexToDestination.OfType<ChildCare>())
            {
                var model = new ChildCareModel();

                model.ChildCareName = cc.Name;
                model.Address = string.Format("{0}, {1}, {2} {3}",
                    cc.Address,
                    cc.City,
                    cc.State,
                    cc.ZipCode);
                model.PhoneNumber = cc.PhoneNumber;
                model.Link = string.Format("http://www.odjfs.state.oh.us/cdc/results2.asp?provider_number={0}", cc.ExternalUrlId);

                var dcc = cc as DetailedChildCare;
                if (dcc != null)
                {
                    model.Hours = new[]
                    {
                        GetBusinessHoursModel(BusinessHoursModel.WeekDay.Sunday, dcc.SundayReported, dcc.SundayBegin, dcc.SundayEnd),
                        GetBusinessHoursModel(BusinessHoursModel.WeekDay.Monday, dcc.MondayReported, dcc.MondayBegin, dcc.MondayEnd),
                        GetBusinessHoursModel(BusinessHoursModel.WeekDay.Tuesday, dcc.TuesdayReported, dcc.TuesdayBegin, dcc.TuesdayEnd),
                        GetBusinessHoursModel(BusinessHoursModel.WeekDay.Wednesday, dcc.WednesdayReported, dcc.WednesdayBegin, dcc.WednesdayEnd),
                        GetBusinessHoursModel(BusinessHoursModel.WeekDay.Thursday, dcc.ThursdayReported, dcc.ThursdayBegin, dcc.ThursdayEnd),
                        GetBusinessHoursModel(BusinessHoursModel.WeekDay.Friday, dcc.FridayReported, dcc.FridayBegin, dcc.FridayEnd),
                        GetBusinessHoursModel(BusinessHoursModel.WeekDay.Saturday, dcc.SaturdayReported, dcc.SaturdayBegin, dcc.SaturdayEnd)
                    }.Where(m => m != null).ToArray();
                }

                yield return model;
            }
        }

        private static BusinessHoursModel GetBusinessHoursModel(BusinessHoursModel.WeekDay weekDay, bool reported, DateTime? startTime, DateTime? endTime)
        {
            if (reported && startTime.HasValue && endTime.HasValue)
            {
                return new BusinessHoursModel
                {
                    Day = weekDay,
                    OpeningTime = startTime.Value.ToString("h:mm tt"),
                    ClosingTime = endTime.Value.ToString("h:mm tt")
                };
            }
            return null;
        }

        private IEnumerable<ChildCareRoute> GetChildCareRoutes()
        {
            // get all destinations and create an index for each
            IDictionary<IDestination, string> destinationToKey = _indexToDestination
                .Select((d, i) => new {Destination = d, Key = i.ToString(CultureInfo.InvariantCulture)})
                .ToDictionary(t => t.Destination, t => t.Key);

            // create a key for each search result, based on the destinations in that search results
            Dictionary<string, SearchResult> dropOffDestinations = _dropOffSearchResults.ToDictionary(sr => GetSearchResultDestinationsKey(destinationToKey, sr));
            Dictionary<string, SearchResult> pickUpDestinations = _pickUpSearchResults.ToDictionary(sr => GetSearchResultDestinationsKey(destinationToKey, sr));

            // associate drop off and pick up search results based off of the key
            foreach (var pair in dropOffDestinations)
            {
                SearchResult pickUpSearchResult;
                if (pickUpDestinations.TryGetValue(pair.Key, out pickUpSearchResult))
                {
                }
                yield return new ChildCareRoute {DropOffSearchResult = pair.Value, PickUpSearchResult = pickUpSearchResult};
            }
            foreach (var pair in pickUpDestinations)
            {
                SearchResult dropOffSearchResult;
                if (!dropOffDestinations.TryGetValue(pair.Key, out dropOffSearchResult))
                {
                    yield return new ChildCareRoute { DropOffSearchResult = null, PickUpSearchResult = pair.Value };   
                }
            }
        }

        private class ChildCareRoute
        {
            public SearchResult DropOffSearchResult { get; set; }
            public SearchResult PickUpSearchResult { get; set; }
        }
    }
}