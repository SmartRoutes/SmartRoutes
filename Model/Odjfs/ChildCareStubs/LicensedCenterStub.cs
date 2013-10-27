using Model.Odjfs.ChildCares;

namespace Model.Odjfs.ChildCareStubs
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