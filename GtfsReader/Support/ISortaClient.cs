using System.Threading.Tasks;
using SmartRoutes.Scraper;

namespace SmartRoutes.SortaScraper.Support
{
    public interface ISortaClient
    {
        Task<ClientResponseHeaders> GetArchiveHeaders();
        Task<ClientResponse> GetArchiveContent();
    }
}