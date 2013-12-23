using System.Threading.Tasks;
using SmartRoutes.GtfsReader.Support;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.GtfsReader.Readers
{
    public interface IGtfsCollectionReader
    {
        Task<GtfsCollection> Download(GtfsArchive currentArchive);
    }
}