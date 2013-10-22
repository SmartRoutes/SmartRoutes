namespace Model.Odjfs
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