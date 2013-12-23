using SmartRoutes.Model;

namespace SmartRoutes.Reader
{
    public abstract class EntityCollection
    {
        public Archive Archive { get; set; }
        public bool ContainsEntities { get; set; }
    }
}