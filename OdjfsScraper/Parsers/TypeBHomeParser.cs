using System.Collections.Generic;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Parsers
{
    public class TypeBHomeParser : BaseChildCareParser<TypeBHome>
    {
        protected override void PopulateFields(TypeBHome childCare, IDictionary<string, string> details)
        {
            // populate the base fields
            base.PopulateFields(childCare, details);

            childCare.CertificationBeginDate = GetDetailString(details, "Certification Begin Date");
            childCare.CertificationExpirationDate = GetDetailString(details, "Certification Expiration Date");
        }
    }
}