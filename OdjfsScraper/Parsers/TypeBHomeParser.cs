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

            // type B homes do not have their address exposed
            // TODO: verify the expected placeholder
            childCare.Address = null;

            childCare.CertificationBeginDate = GetDetailString(details, "Certification Begin Date");
            childCare.CertificationExpirationDate = GetDetailString(details, "Certification Expiration Date");
        }
    }
}