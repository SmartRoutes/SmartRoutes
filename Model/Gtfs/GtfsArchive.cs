namespace SmartRoutes.Model.Gtfs
{
    public class GtfsArchive : Archive
    {
        public const string Discriminator = "GtfsArchive";

        public GtfsArchive()
        {
            ArchiveType = Discriminator;
        }
    }
}