using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Graph;
using SmartRoutes.Graph.Node;
using SmartRoutes.Model;
using SmartRoutes.Models;
using SmartRoutes.Models.Itinerary;

namespace SmartRoutes.Support
{
    public class ChildCareSearchResultsModelBuilder
    {
        private ChildCareModel[] _childCareModels;
        private ChildCareRoute[] _childCareRoutes;
        private ChildCareRouteModel[] _chldCareRouteModels;
        private Func<IDestination, bool>[] _criteria;
        private string _dropOffDepartureAddress;
        private string _dropOffDestinationAddress;
        private SearchResult[] _dropOffSearchResults;
        private IDestination[] _indexToDestination;
        private string _pickUpDepartureAddress;
        private string _pickUpDestinationAddress;
        private SearchResult[] _pickUpSearchResults;
        private IDictionary<IDestination, int> _destinationToIndex;

        private static string GetSearchResultDestinationsKey(IDictionary<IDestination, string> destinationToKey, SearchResult searchResult)
        {
            string destinationsKey = string.Join("-", searchResult
                .Destinations
                .Select(d => destinationToKey[d])
                .OrderBy(k => k));

            return destinationsKey;
        }

        public ChildCareSearchResultsModel Build(
            string dropOffDepartureAddress, string dropOffDestinationAddress,
            string pickUpDepartureAddress, string pickUpDestinationAddress,
            IEnumerable<Func<IDestination, bool>> criteria,
            IEnumerable<SearchResult> pickUpSearchResults, IEnumerable<SearchResult> dropOffSearchResults)
        {
            // set the build parameters
            _dropOffDepartureAddress = dropOffDepartureAddress;
            _dropOffDestinationAddress = dropOffDestinationAddress;
            _pickUpDepartureAddress = pickUpDepartureAddress;
            _pickUpDestinationAddress = pickUpDestinationAddress;
            _criteria = criteria.ToArray();
            _pickUpSearchResults = pickUpSearchResults.ToArray();
            _dropOffSearchResults = dropOffSearchResults.ToArray();

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

        private static void CollectChildCareIndices(ItineraryModel model, ISet<int> childCareIndices)
        {
            foreach (IChildItineraryAction action in model.ItineraryActions.OfType<IChildItineraryAction>())
            {
                childCareIndices.UnionWith(action.ChildIndices);
            }
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
            string departureAddress = model is DropOffItineraryModel ? _dropOffDepartureAddress : _pickUpDepartureAddress;
            string destinationAddress = model is DropOffItineraryModel ? _dropOffDestinationAddress : _pickUpDestinationAddress;

            IList<string> routes = new List<string>();

            int? previousTripId = null;
            model.AddAction(new DepartAction(departureAddress));
            foreach (NodeInfo current in searchResult.ShortResults)
            {
                var currentGtfs = current.Node as IGtfsNode;
                var currentDestination = current.Node as IDestinationNode;

                if (currentGtfs != null)
                {
                    if (currentGtfs.TripId != previousTripId)
                    {
                        model.AddAction(new BoardBusAction(
                            currentGtfs.stopTime.Trip.Headsign ?? currentGtfs.stopTime.Trip.Route.ShortName,
                            current.Node.Time,
                            currentGtfs.stopTime.Stop.Name));

                        routes.Add(currentGtfs.stopTime.Trip.Route.ShortName);
                    }
                    else
                    {
                        model.AddAction(new ExitBusAction(
                            current.Node.Time,
                            currentGtfs.stopTime.Stop.Name));
                    }

                    previousTripId = currentGtfs.TripId;
                }
                else
                {
                    previousTripId = null;

                    if (currentDestination != null)
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
                            model.AddAction(new DropOffAction(childIndices, name));
                        }
                        else
                        {
                            model.AddAction(new PickUpAction(childIndices, name));
                        }
                    }
                }
            }

            model.AddAction(new ArriveAction(destinationAddress));
            model.Routes = routes;
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
                if (dropOffDestinations.TryGetValue(pair.Key, out dropOffSearchResult))
                {
                }
                yield return new ChildCareRoute {DropOffSearchResult = dropOffSearchResult, PickUpSearchResult = pair.Value};
            }
        }

        private class ChildCareRoute
        {
            public SearchResult DropOffSearchResult { get; set; }
            public SearchResult PickUpSearchResult { get; set; }
        }
    }
}