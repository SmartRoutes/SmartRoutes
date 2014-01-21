using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartRoutes.Models;

namespace SmartRoutes.Controllers
{
    public class GuidedSearchPageController : Controller
    {
        //
        // GET: /GuidedSearchPage/

        public ActionResult AccreditationFormPageView()
        {
            // This stuff should probably go in a database eventually,
            // but as there is still debate about database issues, this
            // is just done inline for now.
            List<AccreditationModel> accreditations = new List<AccreditationModel>
            {
                new AccreditationModel("NAEYC", "NAEYC Description", new Uri("http://www.naeyc.org/")),
                new AccreditationModel("NECPA", "NECPA Description", new Uri("http://www.necpa.net/")),
                new AccreditationModel("NACCP", "NACCP Description", new Uri("http://www.earlylearningleaders.org/")),
                new AccreditationModel("NAFCC", "NAFCC Description", new Uri("http://nafcc.org/")),
                new AccreditationModel("COA", "COA Description", new Uri("http://www.childrenofamerica.com/")),
                new AccreditationModel("ACSI", "ACSI Description", new Uri("http://www.acsiglobal.org/")),
                new AccreditationModel("CCFP", "CCFP Description", new Uri("http://www.4c.org/provider/participate/food/overview.html"))
            };

            return PartialView("~/Views/Search/_AccreditationFormPageView.cshtml", accreditations);
        }
    }
}
