using SmartRoutes.Model;
using SmartRoutes.Reader.Support;

namespace SmartRoutes.Reader.Parsers
{
    public interface IEntityCollectionParser<TArchive, out TCollection>
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        TCollection Parse(byte[] bytes);
    }
}