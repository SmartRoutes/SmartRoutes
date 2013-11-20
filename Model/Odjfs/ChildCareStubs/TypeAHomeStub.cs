using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.Model.Odjfs.ChildCareStubs
{
    public class TypeAHomeStub : ChildCareStub
    {
        public const string Discriminator = TypeAHome.DetailedDiscriminator;

        public override string ChildCareType
        {
            get { return Discriminator; }
        }
    }
}