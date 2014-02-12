using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SmartRoutes.Models;
using SmartRoutes.Models.Payloads;

namespace SmartRoutes.Controllers
{
    public class GuidedSearchPageController : Controller
    {
        //
        // GET: /GuidedSearchPage/

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

        public ActionResult AccreditationView()
        {
            return PartialView("~/Views/Search/_AccreditationView.cshtml");
        }

        public ActionResult ServiceTypeView()
        {
            return PartialView("~/Views/Search/_ServiceTypeView.cshtml");
        }

        public JsonResult PerformChildCareSearch(ChildCareSearchQueryPayload searchQuery)
        {
            // This needs to be populated by the search results.
            List<ChildCareSearchResultModel> results = new List<ChildCareSearchResultModel>();

            return Json(results, JsonRequestBehavior.AllowGet); 
        }
    }
}
