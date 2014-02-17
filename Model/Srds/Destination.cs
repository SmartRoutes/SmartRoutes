using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartRoutes.Model.Srds
{
    public class Destination : IDestination
    {
        private IDictionary<string, object> _attributeValueDictionary;
        private ICollection<AttributeValue> _attributeValues;

        public Destination()
        {
            _attributeValues = new Collection<AttributeValue>();
        }

        public int Id { get; set; }

        public virtual ICollection<AttributeValue> AttributeValues
        {
            get { return _attributeValues; }
            set { _attributeValues = value; }
        }

        public string Name { get; set; }

        public DateTime? SundayEnd
        {
            get { return GetAttributeValue<DateTime?>("SundayEnd"); }
        }

        public DateTime? SundayBegin
        {
            get { return GetAttributeValue<DateTime?>("SundayBegin"); }
        }

        public bool SundayReported
        {
            get { return GetAttributeValue<bool>("SundayReported"); }
        }

        public DateTime? SaturdayBegin
        {
            get { return GetAttributeValue<DateTime?>("SaturdayBegin"); }
        }

        public bool SaturdayReported
        {
            get { return GetAttributeValue<bool>("SaturdayReported"); }
        }

        public DateTime? SaturdayEnd
        {
            get { return GetAttributeValue<DateTime?>("SaturdayEnd"); }
        }

        public DateTime? FridayEnd
        {
            get { return GetAttributeValue<DateTime?>("FridayEnd"); }
        }

        public DateTime? FridayBegin
        {
            get { return GetAttributeValue<DateTime?>("FridayBegin"); }
        }

        public bool FridayReported
        {
            get { return GetAttributeValue<bool>("FridayReported"); }
        }

        public DateTime? ThursdayEnd
        {
            get { return GetAttributeValue<DateTime?>("ThursdayEnd"); }
        }

        public DateTime? ThursdayBegin
        {
            get { return GetAttributeValue<DateTime?>("ThursdayBegin"); }
        }

        public bool ThursdayReported
        {
            get { return GetAttributeValue<bool>("ThursdayReported"); }
        }

        public DateTime? WednesdayEnd
        {
            get { return GetAttributeValue<DateTime?>("WednesdayEnd"); }
        }

        public DateTime? TuesdayEnd
        {
            get { return GetAttributeValue<DateTime?>("TuesdayEnd"); }
        }

        public DateTime? TuesdayBegin
        {
            get { return GetAttributeValue<DateTime?>("TuesdayBegin"); }
        }

        public bool TuesdayReported
        {
            get { return GetAttributeValue<bool>("TuesdayReported"); }
        }

        public bool WednesdayReported
        {
            get { return GetAttributeValue<bool>("WednesdayReported"); }
        }

        public DateTime? WednesdayBegin
        {
            get { return GetAttributeValue<DateTime?>("WednesdayBegin"); }
        }

        public DateTime? MondayEnd
        {
            get { return GetAttributeValue<DateTime?>("MondayEnd"); }
        }

        public DateTime? MondayBegin
        {
            get { return GetAttributeValue<DateTime?>("MondayBegin"); }
        }

        public bool MondayReported
        {
            get { return GetAttributeValue<bool>("MondayReported"); }
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public void InitializeAttributeValueDictionary(bool force = false)
        {
            if (_attributeValueDictionary == null || force)
            {
                _attributeValueDictionary = new Dictionary<string, object>();
                foreach (AttributeValue attributeValue in AttributeValues)
                {
                    _attributeValueDictionary[attributeValue.AttributeKey.Name] = attributeValue.Value;
                }
            }
        }

        private T GetAttributeValue<T>(string name)
        {
            // TODO: test... :)
            InitializeAttributeValueDictionary();
            object value;
            if (!_attributeValueDictionary.TryGetValue(name, out value) || !(value is T))
            {
                return default(T);
            }

            return (T) value;
        }
    }
}