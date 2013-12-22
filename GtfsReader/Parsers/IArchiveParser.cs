using SmartRoutes.Model.Gtfs;
using SmartRoutes.Scraper;

namespace SmartRoutes.SortaScraper.Parsers
{
    public interface IArchiveParser
    {
        Archive Parse(ClientResponseHeaders headers);
    }
}