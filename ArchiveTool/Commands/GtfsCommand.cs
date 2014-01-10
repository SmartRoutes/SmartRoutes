using SmartRoutes.ArchiveLoader;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.ArchiveTool.Commands
{
    public class GtfsCommand : ArchiveCommand<GtfsArchive, GtfsCollection>
    {
        public GtfsCommand(IArchiveLoader<GtfsArchive, GtfsCollection> archiveLoader) : base("gtfs", archiveLoader)
        {
        }
    }
}