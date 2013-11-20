using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.Model.Odjfs.ChildCareStubs
{
    public class LicensedCenterStub : ChildCareStub
    {
        public const string Discriminator = LicensedCenter.DetailedDiscriminator;

        public override string ChildCareType
        {
            get { return Discriminator; }
        }
    }
}