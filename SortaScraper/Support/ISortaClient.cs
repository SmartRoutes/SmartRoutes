using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmartRoutes.SortaScraper.Support
{
    public interface ISortaClient
    {
        Task<HttpResponseHeaders> GetArchiveHeaders();
        Task<HttpResponseMessage> GetArchiveContent();
    }
}