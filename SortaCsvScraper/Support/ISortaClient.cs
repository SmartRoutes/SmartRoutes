using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SortaCsvScraper.Support
{
    public interface ISortaClient
    {
        Task<HttpResponseHeaders> GetArchiveHeaders();
        Task<byte[]> GetArchiveBytes();
    }
}