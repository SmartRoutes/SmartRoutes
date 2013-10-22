namespace Model.Odjfs
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