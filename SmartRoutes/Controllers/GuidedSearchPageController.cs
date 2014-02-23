using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.Demo.OdjfsDatabase;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using SmartRoutes.Graph;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;
using SmartRoutes.Models;
using SmartRoutes.Models.Payloads;
using SmartRoutes.Reader.Readers;
using SmartRoutes.Models.Itinerary;

namespace SmartRoutes.Controllers
{
    /// <summary>
    /// Controller that handles requests from the guided search page.
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
                .FromAssemblyContaining(typeof(GtfsCollection), typeof(IEntityCollectionDownloader<,>))
                .SelectAllClasses()
                .BindAllInterfaces());

            // get Metro models
            var gtfsFetcher = kernel.Get<IEntityCollectionDownloader<GtfsArchive, GtfsCollection>>();
            var gtfsCollection = gtfsFetcher.Download(new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"), null).Result;

            // get child care models
            var odjfsDatabase = new OdjfsDatabase("OdjfsDatabase");
            var childCares = odjfsDatabase.GetChildCares().Result;

            // build the graph
            var graphBuilder = kernel.Get<IGraphBuilder>();
            return graphBuilder.BuildGraph(gtfsCollection.StopTimes, childCares, GraphBuilderSettings.Default);
        }

        //
        // GET: /GuidedSearchPage/

        /// <summary>
        /// Retrieves information about the available accreditations in JSON format.
        /// </summary>
        /// <returns>The JSON data.</returns>
        public JsonResult Accreditations()
        {
            // This stuff should probably go in a database eventually,
            // but as there is still debate about database issues, this
            // is just done inline for now.
            List<AccreditationModel> accreditations = new List<AccreditationModel>
            {
                new AccreditationModel(Resources.NAEYCName, Resources.NAEYCDescription, new Uri(Resources.NAEYCURL)),
                new AccreditationModel(Resources.NECPAName, Resources.NECPADescription, new Uri(Resources.NECPAURL)),
                new AccreditationModel(Resources.NACCPName, Resources.NACCPDescription, new Uri(Resources.NACCPURL)),
                new AccreditationModel(Resources.NAFCCName, Resources.NAFCCDescription, new Uri(Resources.NAFCCURL)),
                new AccreditationModel(Resources.COAName, Resources.COADescription, new Uri(Resources.COAURL)),
                new AccreditationModel(Resources.ACSIName, Resources.ACSIDescription, new Uri(Resources.ACSIURL)),
                new AccreditationModel(Resources.CCFPName, Resources.CCFPDescription, new Uri(Resources.CCFPURL))
            };

            return Json(accreditations, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns information about the child care service types in JSON format.
        /// </summary>
        /// <returns>JSON information.</returns>
        public JsonResult ServiceTypes()
        {
            List<ServiceTypeModel> serviceTypes = new List<ServiceTypeModel>
            {
                new ServiceTypeModel(Resources.TypeAHomeName, Resources.TypeAHomeDescription),
                // I believe type B homes don't have addresses, so omit those for now.
                //new ServiceTypeModel(Resources.TypeBHomeName, Resources.TypeBHomeDescription),
                new ServiceTypeModel(Resources.LicensedCenterName, Resources.LicensedCenterDescription),
                new ServiceTypeModel(Resources.DayCampName, Resources.DayCampDescription)
            };

            return Json(serviceTypes, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns the raw HTML for the accreditation view.
        /// </summary>
        /// <returns>HTML string for the accreditation view.</returns>
        public ActionResult AccreditationView()
        {
            return PartialView("~/Views/Search/_AccreditationView.cshtml");
        }

        /// <summary>
        /// Returns the raw HTML for the service type view.
        /// </summary>
        /// <returns>HTML string for the service type view.</returns>
        public ActionResult ServiceTypeView()
        {
            return PartialView("~/Views/Search/_ServiceTypeView.cshtml");
        }

        /// <summary>
        /// Performs the child care search for the supplied query and returns
        /// the results.
        /// </summary>
        /// <param name="searchQuery">The query for the search.</param>
        /// <returns>The results of the search.</returns>
        public JsonResult PerformChildCareSearch(ChildCareSearchQueryPayload searchQuery)
        {
            ChildCareSearchResultsModel results = new ChildCareSearchResultsModel();

            // starting at my address
            var homeLocation = new Destination { Latitude = 39.122309, Longitude = -84.507639 };

            // ending at the college of engineering
            var workLocation = new Destination { Latitude = 39.133292, Longitude = -84.515099 };

            if (searchQuery.ScheduleType.DropOffChecked)
            {
                var childInformation = searchQuery.ChildInformation.First();

                Func<IDestination, bool> criterion = destination =>
                {
                    ChildCare childCare = destination as ChildCare;
                    DetailedChildCare detailedChildCare = destination as DetailedChildCare;
                    if (childCare == null)
                    {
                        return false;
                    }

                    if (detailedChildCare != null &&
                        // detect if age groups are reported at all for this child care
                        (detailedChildCare.Infants || detailedChildCare.YoungToddlers || detailedChildCare.OlderToddlers ||
                         detailedChildCare.Gradeschoolers || detailedChildCare.Preschoolers))
                    {
                        if (childInformation.AgeGroup == Resources.AgeGroupInfantName && !detailedChildCare.Infants)
                        {
                            return false;
                        }

                        if (childInformation.AgeGroup == Resources.AgeGroupOlderToddlerName &&
                            !detailedChildCare.OlderToddlers)
                        {
                            return false;
                        }

                        if (childInformation.AgeGroup == Resources.AgeGroupYoungToddlerName &&
                            !detailedChildCare.YoungToddlers)
                        {
                            return false;
                        }

                        if (childInformation.AgeGroup == Resources.AgeGroupSchoolAgeName &&
                            !detailedChildCare.Gradeschoolers)
                        {
                            return false;
                        }

                        if (childInformation.AgeGroup == Resources.AgeGroupPreschoolerName &&
                            !detailedChildCare.Preschoolers)
                        {
                            return false;
                        }
                    }

                    return true;
                };

                IEnumerable<NodeInfo> path = Graph.Search(homeLocation, workLocation, searchQuery.LocationsAndTimes.DropOffLatestArrivalTime,
                    TimeDirection.Backwards, new[] {criterion});
            }

            return Json(results, JsonRequestBehavior.AllowGet); 
        }
    }
}
