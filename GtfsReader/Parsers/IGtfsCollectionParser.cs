using SmartRoutes.GtfsReader.Support;

namespace SmartRoutes.GtfsReader.Parsers
{
    public interface IGtfsCollectionParser
    {
        GtfsCollection Parse(byte[] bytes);
    }
}