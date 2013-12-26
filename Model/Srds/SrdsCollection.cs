using System.Collections.Generic;

namespace SmartRoutes.Model.Srds
{
    public class SrdsCollection : EntityCollection<SrdsArchive>
    {
        public Destination[] Destinations { get; set; }
        public AttributeKey[] AttributeKeys { get; set; }
        public AttributeValue[] AttributeValues { get; set; }
    }
}