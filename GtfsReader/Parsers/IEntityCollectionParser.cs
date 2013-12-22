using SmartRoutes.GtfsReader.Support;

namespace SmartRoutes.GtfsReader.Parsers
{
    public interface IEntityCollectionParser
    {
        EntityCollection Parse(byte[] bytes);
    }
}