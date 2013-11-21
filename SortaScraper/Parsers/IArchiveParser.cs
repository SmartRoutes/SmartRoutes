using System.Net.Http.Headers;
using SmartRoutes.Model.Sorta;

namespace SmartRoutes.SortaScraper.Parsers
{
    public interface IArchiveParser
    {
        Archive Parse(HttpResponseHeaders headers);
    }
}