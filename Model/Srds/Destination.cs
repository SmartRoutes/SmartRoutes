using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Srds
{
    public class Destination : ILocation
    {
        private ICollection<AttributeValue> _attributeValues;

        public Destination()
        {
            _attributeValues = new Collection<AttributeValue>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AttributeValue> AttributeValues
        {
            get { return _attributeValues; }
            set { _attributeValues = value; }
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}