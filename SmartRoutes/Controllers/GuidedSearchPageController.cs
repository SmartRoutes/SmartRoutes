using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SmartRoutes.Models;
using SmartRoutes.Models.Payloads;

namespace SmartRoutes.Controllers
{
    /// <summary>
    /// Controller that handles requests from the guided search page.
    /// </summary>
    public class GuidedSearchPageController : Controller
    {
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

            return null; // Json(results, JsonRequestBehavior.AllowGet); 
        }
    }
}
