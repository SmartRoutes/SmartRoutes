using System.Threading.Tasks;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.GtfsReader.Readers
{
    public interface IGtfsCollectionDownloader
    {
        Task<GtfsCollection> Download(Archive currentArchive);
    }
}