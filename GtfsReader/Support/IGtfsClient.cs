using System.Threading.Tasks;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Support
{
    public interface IGtfsClient
    {
        Task<ClientResponse> GetArchiveContent();
    }
}