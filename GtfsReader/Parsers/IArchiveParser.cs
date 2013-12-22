using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader;

namespace SmartRoutes.GtfsReader.Parsers
{
    public interface IArchiveParser
    {
        Archive Parse(ClientResponseHeaders headers);
    }
}