using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace SmartRoutes.Database.Data
{
    public class Record
    {
        private static readonly IDictionary<Type, Func<object, object>[]> AllPropertyGetters = new Dictionary<Type, Func<object, object>[]>();
        private static readonly IDictionary<Type, ReadOnlyCollection<string>> AllPropertyNames = new Dictionary<Type, ReadOnlyCollection<string>>();
        private static readonly IDictionary<Type, ReadOnlyCollection<Type>> AllPropertyTypes = new Dictionary<Type, ReadOnlyCollection<Type>>();

        protected Record()
        {
        }

        public IEnumerable<string> Names { get; private set; }
        public IEnumerable<Type> Types { get; private set; }
        public IEnumerable<object> Values { get; private set; }

        public static Record Create(object obj)
        {
            // reflect on the type
            Type type = obj.GetType();
            Func<object, object>[] propertyGetters;
            if (!AllPropertyGetters.TryGetValue(type, out propertyGetters))
            {
                PropertyInfo[] propertyInfos = type
                    .GetProperties()
                    .Where(p => !p.GetGetMethod().IsVirtual)
                    .ToArray();

                propertyGetters = propertyInfos
                    .Select(localProperty => (Func<object, object>) (o => localProperty.GetGetMethod().Invoke(obj, null)))
                    .ToArray();
                AllPropertyGetters[type] = propertyGetters;
                AllPropertyNames[type] = propertyInfos
                    .Select(p => p.Name)
                    .ToList()
                    .AsReadOnly();
                AllPropertyTypes[type] = propertyInfos
                    .Select(p => p.PropertyType)
                    .ToList()
                    .AsReadOnly();
            }

            // get the values
            ReadOnlyCollection<object> values = propertyGetters
                .Select(f => f(obj))
                .ToList()
                .AsReadOnly();

            // create the Record
            var propertyList = new Record
            {
                Names = AllPropertyNames[type],
                Types = AllPropertyTypes[type],
                Values = values
            };

            return propertyList;
        }
    }
}