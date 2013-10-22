using System.Collections.Generic;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Parsers
{
    public class TypeBHomeParser : BaseChildCareParser<TypeBHome>
    {
        protected override void PopulateFields(TypeBHome childCare, IDictionary<string, string> details)
        {
            // populate the base fields
            base.PopulateFields(childCare, details);

            childCare.CertificationBeginDate = GetDetailString(details, "Certification Begin Date");
            childCare.CertificationEndDate = GetDetailString(details, "Certification Expiration Date");
        }
    }
}