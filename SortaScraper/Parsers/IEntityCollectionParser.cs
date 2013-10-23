using SortaScraper.Support;

namespace SortaScraper.Parsers
{
    public interface IEntityCollectionParser
    {
        EntityCollection Parse(byte[] bytes);
    }
}