using SmartRoutes.SortaScraper.Support;

namespace SmartRoutes.SortaScraper.Parsers
{
    public interface IEntityCollectionParser
    {
        EntityCollection Parse(byte[] bytes);
    }
}