using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Srds
{
    public class Destination : ILocation
    {
        private ICollection<AttributeValue> _attributes;

        public Destination()
        {
            _attributes = new Collection<AttributeValue>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AttributeValue> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}