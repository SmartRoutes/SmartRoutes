using SortaCsvScraper.Support;

namespace SortaCsvScraper.Parsers
{
    public interface IEntityCollectionParser
    {
        EntityCollection Parse(byte[] bytes);
    }
}