using Model.Odjfs.ChildCares;

namespace Model.Odjfs.ChildCareStubs
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