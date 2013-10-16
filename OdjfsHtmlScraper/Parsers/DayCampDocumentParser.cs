using System;
using System.Collections.Generic;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Parsers
{
    public class DayCampDocumentParser : ChildCareDocumentParser<DayCamp>
    {
        protected override DayCamp PopulateFields(DayCamp childCare, IDictionary<string, string> details)
        {
            childCare.Address = details["Address"];
            childCare.RegistrationStatus = details["Registration Status"];
            childCare.Owner = details["Owner"];
            childCare.RegistrationBeginDate = ParseDate(details["Registration Begin Date"]);
            childCare.RegistrationEndDate = details["Registration End Date"] != "NA" ? ParseDate(details["Registration End Date"]) : (DateTime?) null;

            return childCare;
        }
    }
}