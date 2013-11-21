using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public class DayCampTemplate : BaseChildCareTemplate<DayCamp>
    {
        public DayCampTemplate() : base(new DayCamp())
        {
            Model.EffectiveDates = "EffectiveDates";
            Model.Owner = "Owner";
            Model.RegistrationBeginDate = "RegistrationBeginDate";
            Model.RegistrationEndDate = "RegistrationEndDate";
            Model.RegistrationStatus = "RegistrationStatus";
            Model.EffectiveDates = "EffectiveDates";
        }
    }
}