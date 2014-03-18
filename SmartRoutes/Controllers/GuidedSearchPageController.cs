using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PolyGeocoder.Geocoders;
using PolyGeocoder.Support;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Graph;
using SmartRoutes.Model;
using SmartRoutes.Models;
using SmartRoutes.Models.Itinerary;
using SmartRoutes.Models.Payloads;
using SmartRoutes.Support;
using Location = PolyGeocoder.Support.Location;

namespace SmartRoutes.Controllers
{
    /// <summary>
    ///     Controller that handles requests from the guided search page.
    /// </summary>
    public class GuidedSearchPageController : Controller
    {
        //
        // GET: /GuidedSearchPage/

        /// <summary>
        ///     Retrieves information about the available accreditations in JSON format.
        /// </summary>
        /// <returns>The JSON data.</returns>
        public JsonResult Accreditations()
        {
            return Json(ResourceModels.AccreditationModels, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Returns information about the child care service types in JSON format.
        /// </summary>
        /// <returns>JSON information.</returns>
        public JsonResult ServiceTypes()
        {
            return Json(ResourceModels.ServiceTypeModels, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Returns the raw HTML for the accreditation view.
        /// </summary>
        /// <returns>HTML string for the accreditation view.</returns>
        public ActionResult AccreditationView()
        {
            return PartialView("~/Views/Search/_AccreditationView.cshtml");
        }

        /// <summary>
        ///     Returns the raw HTML for the service type view.
        /// </summary>
        /// <returns>HTML string for the service type view.</returns>
        public ActionResult ServiceTypeView()
        {
            return PartialView("~/Views/Search/_ServiceTypeView.cshtml");
        }

        private static ILocation Geocode(ISimpleGeocoder geocoder, IDictionary<string, ILocation> destinations, string request)
        {
            // try to get a previous response
            ILocation location;
            if (destinations.TryGetValue(request, out location))
            {
                return location;
            }

            // geocode the address and save the result to the dictionary
            Response response = geocoder.GeocodeAsync(request).Result;
            Location geocodedLocation = response.Locations.FirstOrDefault();
            if (geocodedLocation != null)
            {
                location = new Model.Location
                {
                    Latitude = geocodedLocation.Latitude,
                    Longitude = geocodedLocation.Longitude
                };
            }
            else
            {
                return null;
            }
            destinations[request] = location;

            return location;
        }

        private static string GetAddressString(AddressPayload addressPayload)
        {
            if (addressPayload == null)
            {
                return null;
            }

            return string.Join(", ", new[]
            {
                (addressPayload.Address + " " + (addressPayload.AddressLine2 ?? string.Empty)).Trim(),
                addressPayload.City,
                addressPayload.State,
                addressPayload.ZipCode
            });
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

        private DateTime StandardizeTime(DateTime dateTime)
        {
            return new DateTime(
                1970, 1, 1,
                dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }

        /// <summary>
        ///     Performs the child care search for the supplied query and returns
        ///     the results.
        /// </summary>
        /// <param name="searchQuery">The query for the search.</param>
        /// <returns>The results of the search.</returns>
        public JsonResult PerformChildCareSearch(ChildCareSearchQueryPayload searchQuery)
        {
            // create the address strings
            string dropOffDepartureAddress = GetAddressString(searchQuery.LocationsAndTimes.DropOffDepartureAddress);
            string dropOffDestinationAddress = GetAddressString(searchQuery.LocationsAndTimes.DropOffDestinationAddress);
            string pickUpDepartureAddress = GetAddressString(searchQuery.LocationsAndTimes.PickUpDepartureAddress);
            string pickUpDestinationAddress = GetAddressString(searchQuery.LocationsAndTimes.PickUpDestinationAddress);

            // check for shared addresses between the pick up and drop off plans
            if (searchQuery.ScheduleType.DropOffChecked && searchQuery.ScheduleType.PickUpChecked)
            {
                if (searchQuery.LocationsAndTimes.PickUpDepartureAddressSameAsDropOffDestination)
                {
                    pickUpDepartureAddress = dropOffDestinationAddress;
                }

                if (searchQuery.LocationsAndTimes.PickUpDestinationSameAsDropOffDeparture)
                {
                    pickUpDestinationAddress = dropOffDepartureAddress;
                }
            }

            // create the criteria functions
            Func<IDestination, bool>[] criteria = searchQuery
                .ChildInformation
                .Select(childInformation => CreateCriterion(searchQuery, childInformation))
                .ToArray();

            // search for both the drop off and pick up plans
            var geocoder = new OpenStreetMapGeocoder(new Client(), OpenStreetMapGeocoder.MapQuestEndpoint);
            var responses = new Dictionary<string, ILocation>();
            IEnumerable<SearchResult> dropOffResults = Enumerable.Empty<SearchResult>();
            if (searchQuery.ScheduleType.DropOffChecked)
            {
                var destination = Geocode(geocoder, responses, dropOffDestinationAddress);
                var departure = Geocode(geocoder, responses, dropOffDepartureAddress);
                if (departure == null)
                {
                    return Json(new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.DropOffDepartureGeocodeFail)
                    });
                }
                if (destination == null)
                {
                    return Json(new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.DropOffDestinationGeocodeFail)
                    });
                }

                dropOffResults = GraphSingleton.Instance.Graph.Search(
                    destination,
                    departure,
                    StandardizeTime(searchQuery.LocationsAndTimes.DropOffLatestArrivalTime),
                    TimeDirection.Backwards,
                    criteria,
                    10);
            }

            IEnumerable<SearchResult> pickUpResults = Enumerable.Empty<SearchResult>();
            if (searchQuery.ScheduleType.PickUpChecked)
            {
                var departure = Geocode(geocoder, responses, pickUpDepartureAddress);
                var destination = Geocode(geocoder, responses, pickUpDestinationAddress);
                if (departure == null)
                {
                    return Json(new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.PickUpDepartureGeocodeFail)
                    });
                }
                if (destination == null)
                {
                    return Json(new ChildCareSearchResultsModel
                    {
                        Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.PickUpDestinationGeocodeFail)
                    });
                }

                pickUpResults = GraphSingleton.Instance.Graph.Search(
                    departure,
                    destination,
                    StandardizeTime(searchQuery.LocationsAndTimes.PickUpDepartureTime),
                    TimeDirection.Forwards,
                    criteria,
                    10);
            }

            // create the model resprentation of the search results
            var builder = new ChildCareSearchResultsModelBuilder();
            ChildCareSearchResultsModel model = builder.Build(
                dropOffDepartureAddress, dropOffDestinationAddress,
                pickUpDepartureAddress, pickUpDestinationAddress,
                criteria,
                pickUpResults, dropOffResults);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}