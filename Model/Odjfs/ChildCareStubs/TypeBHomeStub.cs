using Model.Odjfs.ChildCares;

namespace Model.Odjfs.ChildCareStubs
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