using System.Collections.Generic;

namespace SmartRoutes.Model.Srds
{
    public class SrdsCollection : EntityCollection<SrdsArchive>
    {
        public IEnumerable<Destination> Destinations { get; set; }
        public IEnumerable<AttributeKey> AttributeKeys { get; set; }
        public IEnumerable<AttributeValue> AttributeValues { get; set; }
    }
}