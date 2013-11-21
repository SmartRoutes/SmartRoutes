using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.Model.Odjfs.ChildCareStubs
{
    public class TypeBHomeStub : ChildCareStub
    {
        public const string Discriminator = TypeBHome.Discriminator;

        public override string ChildCareType
        {
            get { return Discriminator; }
        }
    }
}