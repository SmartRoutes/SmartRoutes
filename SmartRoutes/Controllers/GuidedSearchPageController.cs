using System.Web.Mvc;
using PolyGeocoder.Geocoders;
using PolyGeocoder.Support;
using SmartRoutes.Models;
using SmartRoutes.Models.Payloads;
using SmartRoutes.Support;

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

        /// <summary>
        ///     Performs the child care search for the supplied query and returns
        ///     the results.
        /// </summary>
        /// <param name="searchQuery">The query for the search.</param>
        /// <returns>The results of the search.</returns>
        public JsonResult PerformChildCareSearch(ChildCareSearchQueryPayload searchQuery)
        {
            ChildCareSearchResultsModel model = null;

            try
            {
                var geocoder = new OpenStreetMapGeocoder(new Client(), OpenStreetMapGeocoder.MapQuestEndpoint);

                // create the model resprentation of the search results
                var builder = new ChildCareSearchResultsModelBuilder();
                model = builder.Build(geocoder, searchQuery);
            }
            catch (System.Exception)
            {
                model = new ChildCareSearchResultsModel();
                model.Status = new SearchResultsStatus(SearchResultsStatus.StatusCode.NoResults,
                                                       Resources.ErrorNoResults);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}