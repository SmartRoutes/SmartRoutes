using Model.Odjfs.ChildCares;

namespace Model.Odjfs.ChildCareStubs
{
    public class DayCampStub : ChildCareStub
    {
        public const string Discriminator = DayCamp.Discriminator;

        public override string ChildCareType
        {
            get { return Discriminator; }
        }
    }
}