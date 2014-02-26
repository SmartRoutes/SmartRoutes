using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject;
using Ninject.Extensions.Conventions;
using PolyGeocoder.Geocoders;
using PolyGeocoder.Support;
using SmartRoutes.Demo.OdjfsDatabase;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Graph;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;
using SmartRoutes.Models;
using SmartRoutes.Models.Payloads;
using SmartRoutes.Reader.Readers;
using SmartRoutes.Support;
using Location = PolyGeocoder.Support.Location;

namespace SmartRoutes.Controllers
{
    /// <summary>
    ///     Controller that handles requests from the guided search page.
    /// </summary>
    public class GuidedSearchPageController : Controller
    {
        private static IGraph _graph;

        public static IGraph Graph
        {
            get { return _graph ?? (_graph = BuildGraph()); }
        }

        private static IGraph BuildGraph()
        {
            IKernel kernel = new StandardKernel(new GraphModule());

            kernel.Bind(c => c
                .FromAssemblyContaining(typeof (GtfsCollection), typeof (IEntityCollectionDownloader<,>))
                .SelectAllClasses()
                .BindAllInterfaces());

            // get Metro models
            var gtfsFetcher = kernel.Get<IEntityCollectionDownloader<GtfsArchive, GtfsCollection>>();
            GtfsCollection gtfsCollection =
                gtfsFetcher.Download(new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"), null)
                    .Result;

            // get child care models
            var odjfsDatabase = new OdjfsDatabase("OdjfsDatabase");
            IEnumerable<ChildCare> childCares = odjfsDatabase.GetChildCares().Result;

            // build the graph
            var graphBuilder = kernel.Get<IGraphBuilder>();
            return graphBuilder.BuildGraph(gtfsCollection.StopTimes, childCares, GraphBuilderSettings.Default);
        }

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

        private static async Task<Destination> Geocode(ISimpleGeocoder geocoder,
            IDictionary<string, Destination> destinations, AddressPayload addressPayload)
        {
            // convert the request to a string
            string request = string.Join(", ", new[]
            {
                addressPayload.Address,
                addressPayload.AddressLine2,
                addressPayload.City,
                addressPayload.State,
                addressPayload.ZipCode
            });

            // try to get a previous response
            Destination destination;
            if (destinations.TryGetValue(request, out destination))
            {
                return destination;
            }

            // geocode the address and save the result to the dictionary
            Response response = await geocoder.GeocodeAsync(request);
            Location location = response.Locations.FirstOrDefault();
            if (location != null)
            {
                destination = new Destination
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Name = location.Name
                };
            }
            else
            {
                throw new GeocoderException(
                    string.Format("The provivided address '{0}' could not be geocoded using geocode '{1}'.",
                        request,
                        geocoder.GetType().FullName));
            }
            destinations[request] = destination;

            return destination;
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
                    var checkedAccreditations = searchQuery
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

        /// <summary>
        ///     Performs the child care search for the supplied query and returns
        ///     the results.
        /// </summary>
        /// <param name="searchQuery">The query for the search.</param>
        /// <returns>The results of the search.</returns>
        public async Task<JsonResult> PerformChildCareSearchAsync(ChildCareSearchQueryPayload searchQuery)
        {
            var results = new ChildCareSearchResultsModel();

            // geocode the start and end location
            var geocoder = new OpenStreetMapGeocoder(new Client(), OpenStreetMapGeocoder.MapQuestEndpoint);
            var responses = new Dictionary<string, Destination>();

            if (searchQuery.ScheduleType.DropOffChecked)
            {
                Destination startLocation =
                    await Geocode(geocoder, responses, searchQuery.LocationsAndTimes.DropOffDepartureAddress);
                Destination endLocation =
                    await Geocode(geocoder, responses, searchQuery.LocationsAndTimes.DropOffDestinationAddress);

                Func<IDestination, bool>[] criteria = searchQuery
                    .ChildInformation
                    .Select(childInformation => CreateCriterion(searchQuery, childInformation))
                    .ToArray();

                IEnumerable<NodeInfo> path = Graph.Search(
                    startLocation,
                    endLocation,
                    searchQuery.LocationsAndTimes.DropOffLatestArrivalTime,
                    TimeDirection.Backwards,
                    criteria);
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}