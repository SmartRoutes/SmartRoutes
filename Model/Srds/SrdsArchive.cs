namespace SmartRoutes.Model.Srds
{
    public class SrdsArchive : Archive
    {
        public const string Discriminator = "SrdsArchive";

        public SrdsArchive()
        {
            ArchiveType = Discriminator;
        }
    }
}