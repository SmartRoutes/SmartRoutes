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
                throw new GeocoderException(
                    string.Format("The provivided address '{0}' could not be geocoded using geocode '{1}'.",
                        request,
                        geocoder.GetType().FullName));
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
                if (!ResourceModels.ServiceTypeValidators.Values.Any(validator => validator(childCare)))
                {
                    return false;
                }

                return true;
            };
        }

        private static ChildCareSearchResultsModel GetSpoofedSearchResults()
        {
            var results = new ChildCareSearchResultsModel();

            results.AddChildCare(new ChildCareModel
            {
                Address = "47 CORRY BOULEVARD, CINCINNATI, OH, 45221",
                ChildCareName = "ARLITT CHILD DEVELOPMENT CENTER",
                Hours = new BusinessHoursModel[0],
                Link = "http://www.google.com/#q=test",
                PhoneNumber = "513-556-3802",
                ReviewLink = "http://www.google.com/#q=review"
            });

            /*
            results.AddChildCare(new ChildCareModel
            {
                Address = "6601 HAMILTON, CINCINNATI, OH, 45224",
                ChildCareName = "ANGELS OF JOY CHILDREN LEARNING CENTER",
                Hours = new[]
                {
                    new BusinessHoursModel {Day = BusinessHoursModel.WeekDay.Monday, OpeningTime = "6:00 AM", ClosingTime = "11:55 PM"},
                    new BusinessHoursModel {Day = BusinessHoursModel.WeekDay.Tuesday, OpeningTime = "6:00 AM", ClosingTime = "11:55 PM"},
                    new BusinessHoursModel {Day = BusinessHoursModel.WeekDay.Wednesday, OpeningTime = "6:00 AM", ClosingTime = "11:55 PM"},
                    new BusinessHoursModel {Day = BusinessHoursModel.WeekDay.Thursday, OpeningTime = "6:00 AM", ClosingTime = "11:55 PM"},
                    new BusinessHoursModel {Day = BusinessHoursModel.WeekDay.Friday, OpeningTime = "6:00 AM", ClosingTime = "11:55 PM"}
                },
                Link = null,
                PhoneNumber = null,
                ReviewLink = null
            });
            results.AddChildCare(new ChildCareModel
            {
                Address = "1655 CHASE AVENUE, CINCINNATI, OH, 45223",
                ChildCareName = "AMICUS CHILDREN LEARNING CENTER MCKIE",
                Hours = new BusinessHoursModel[0],
                Link = null,
                PhoneNumber = "513-541-5300",
                ReviewLink = null
            });
            */

            var dropOff = new DropOffItineraryModel();
            dropOff.AddAction(new DepartAction("2300 Stratford Ave, Cincinnati, OH 45219"));
            dropOff.AddAction(new BoardBusAction("31", new DateTime(1970, 1, 1, 8, 13, 00), "Mcmillan St & Chickasaw St"));
            dropOff.AddAction(new ExitBusAction(new DateTime(1970, 1, 1, 8, 25, 0), "Mcmillan St & Scioto St"));
            dropOff.AddAction(new DropOffAction(new[] { 0 }, "ARLITT CHILD DEVELOPMENT CENTER"));
            dropOff.AddAction(new BoardBusAction("32", new DateTime(1970, 1, 1, 8, 47, 0), "Mcmillan St & Scioto St"));
            dropOff.AddAction(new ExitBusAction(new DateTime(1970, 1, 1, 9, 5, 0), "Mcmillan St & Symmes St"));
            dropOff.AddAction(new ArriveAction("499 E McMillan St, Cincinnati, OH 45206"));
            dropOff.Routes = new[] { "31", "32" };

            var pickUp = new PickUpItineraryModel();
            pickUp.AddAction(new DepartAction("499 E McMillan St, Cincinnati, OH 45206"));
            pickUp.AddAction(new BoardBusAction("33", new DateTime(1970, 1, 1, 17, 3, 00), "Mcmillan St & Symmes St"));
            pickUp.AddAction(new ExitBusAction(new DateTime(1970, 1, 1, 17, 20, 0), "Mcmillan St & Scioto St"));
            pickUp.AddAction(new PickUpAction(new[] { 0 }, "ARLITT CHILD DEVELOPMENT CENTER"));
            pickUp.AddAction(new BoardBusAction("34", new DateTime(1970, 1, 1, 17, 49, 0), "Mcmillan St & Scioto St"));
            pickUp.AddAction(new ExitBusAction(new DateTime(1970, 1, 1, 18, 2, 0), "Mcmillan St & Chickasaw St"));
            pickUp.AddAction(new ArriveAction("2300 Stratford Ave, Cincinnati, OH 45219"));
            pickUp.Routes = new[] { "33" , "34" };

            results.AddChildCareRoute(new ChildCareRouteModel
            {
                ResultPriority = 0,
                ChildCareIndices = new[] { 0 },
                DropOffPlan = dropOff,
                PickUpPlan = pickUp
            });

            return results;
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
                dropOffResults = GraphSingleton.Instance.Graph.Search(
                    Geocode(geocoder, responses, dropOffDestinationAddress),
                    Geocode(geocoder, responses, dropOffDepartureAddress),
                    StandardizeTime(searchQuery.LocationsAndTimes.DropOffLatestArrivalTime),
                    TimeDirection.Backwards,
                    criteria);
            }

            IEnumerable<SearchResult> pickUpResults = Enumerable.Empty<SearchResult>();
            if (searchQuery.ScheduleType.PickUpChecked)
            {
                pickUpResults = GraphSingleton.Instance.Graph.Search(
                    Geocode(geocoder, responses, pickUpDepartureAddress),
                    Geocode(geocoder, responses, pickUpDestinationAddress),
                    StandardizeTime(searchQuery.LocationsAndTimes.PickUpDepartureTime),
                    TimeDirection.Forwards,
                    criteria);
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