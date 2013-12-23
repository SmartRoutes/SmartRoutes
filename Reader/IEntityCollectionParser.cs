namespace SmartRoutes.Reader
{
    public interface IEntityCollectionParser<T> where T : EntityCollection
    {
        T Parse(byte[] bytes);
    }
}