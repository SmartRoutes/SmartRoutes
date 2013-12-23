using System.Collections.Generic;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader;

namespace SmartRoutes.SrdsReader.Readers
{
    public class SrdsCollection : EntityCollection
    {
        public IEnumerable<Destination> Destinations { get; set; }
        public IEnumerable<AttributeKey> AttributeKeys { get; set; }
        public IEnumerable<AttributeValue> AttributeValues { get; set; }
    }
}