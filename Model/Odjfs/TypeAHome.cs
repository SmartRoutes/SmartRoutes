namespace Model.Odjfs
{
    public class TypeAHome : DetailedChildCare
    {
        public const string DetailedDiscriminator = "TypeAHome";

        public TypeAHome()
        {
            DetailedChildCareType = DetailedDiscriminator;
        }
    }
}