namespace Model.Odjfs
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