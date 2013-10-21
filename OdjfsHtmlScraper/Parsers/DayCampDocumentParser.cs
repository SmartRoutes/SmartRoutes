using System.Collections.Generic;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Parsers
{
    public class DayCampDocumentParser : ChildCareDocumentParser<DayCamp>
    {
        protected override DayCamp PopulateFields(DayCamp childCare, IDictionary<string, string> details)
        {
            // populate the base fields
            base.PopulateFields(childCare, details);

            childCare.Address = GetDetailString(details, "Address");
            childCare.RegistrationStatus = GetDetailString(details, "Registration Status");
            childCare.Owner = GetDetailString(details, "Owner");
            childCare.RegistrationBeginDate = GetDetailString(details, "Registration Begin Date");
            childCare.RegistrationEndDate = GetDetailString(details, "Registration End Date");
            childCare.EffectiveDates = GetDetailString(details, "Effective Dates");

            return childCare;
        }
    }
}