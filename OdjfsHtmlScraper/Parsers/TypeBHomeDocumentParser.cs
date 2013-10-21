using System.Collections.Generic;
using Model.Odjfs;

namespace OdjfsHtmlScraper.Parsers
{
    public class TypeBHomeDocumentParser : ChildCareDocumentParser<TypeBHome>
    {
        protected override TypeBHome PopulateFields(TypeBHome childCare, IDictionary<string, string> details)
        {
            // populate the base fields
            base.PopulateFields(childCare, details);

            childCare.CertificationBeginDate = GetDetailString(details, "Certification Begin Date");
            childCare.CertificationEndDate = GetDetailString(details, "Certification Expiration Date");

            return childCare;
        }
    }
}