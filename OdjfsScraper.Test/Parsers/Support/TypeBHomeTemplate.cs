using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public class TypeBHomeTemplate : BaseChildCareTemplate<TypeBHome>
    {
        public TypeBHomeTemplate()
        {
            Model.CertificationBeginDate = "CertificationBeginDate";
            Model.CertificationExpirationDate = "CertificationExpirationDate";
        }
    }
}