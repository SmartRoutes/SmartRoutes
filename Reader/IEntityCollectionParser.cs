using SmartRoutes.Model;

namespace SmartRoutes.Reader
{
    public interface IEntityCollectionParser<TArchive, out TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        TCollection Parse(byte[] bytes);
    }
}