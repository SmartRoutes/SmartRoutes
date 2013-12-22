using System.Threading.Tasks;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Support
{
    public interface ISortaClient
    {
        Task<ClientResponseHeaders> GetArchiveHeaders();
        Task<ClientResponse> GetArchiveContent();
    }
}