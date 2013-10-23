using System.Net.Http.Headers;
using Model.Sorta;

namespace SortaScraper.Parsers
{
    public interface IArchiveParser
    {
        Archive Parse(HttpResponseHeaders headers);
    }
}