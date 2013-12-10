using SmartRoutes.Model.Sorta;
using SmartRoutes.Scraper;

namespace SmartRoutes.SortaScraper.Parsers
{
    public interface IArchiveParser
    {
        Archive Parse(ClientResponseHeaders headers);
    }
}