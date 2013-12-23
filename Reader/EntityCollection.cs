using SmartRoutes.Model;

namespace SmartRoutes.Reader
{
    public abstract class EntityCollection<TArchive> where TArchive : Archive
    {
        public TArchive Archive { get; set; }
        public bool ContainsEntities { get; set; }
    }
}