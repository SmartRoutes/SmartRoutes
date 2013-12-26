using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using SmartRoutes.Model.Gtfs;

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

        public Type UnderlyingType { get; private set; }
        public IEnumerable<string> Names { get; private set; }
        public IEnumerable<Type> Types { get; private set; }
        public IEnumerable<object> Values { get; private set; }

        public static IEnumerable<string> GetPropertyNames(Type type)
        {
            Initialize(type);
            return AllPropertyNames[type];
        }

        public static IEnumerable<Type> GetPropertyTypes(Type type)
        {
            Initialize(type);
            return AllPropertyTypes[type];
        }

        public static void Initialize(Type type)
        {
            // get the properties
            PropertyInfo[] propertyInfos = type
                .GetProperties()
                .Where(p => !p.GetGetMethod().IsVirtual || p.GetGetMethod().IsFinal)
                .ToArray();

            // store some info
            AllPropertyGetters[type] = propertyInfos
                .Select(p => (Func<object, object>)(p.GetValue))
                .ToArray();
            AllPropertyNames[type] = propertyInfos
                .Select(p => p.Name)
                .ToList()
                .AsReadOnly();
            AllPropertyTypes[type] = propertyInfos
                .Select(p => p.PropertyType)
                .ToList()
                .AsReadOnly();
        }

        public static Record Create(object obj)
        {
            // reflect on the type
            Type underlyingType = obj.GetType();
            Initialize(underlyingType);

            // get the values
            ReadOnlyCollection<object> values = AllPropertyGetters[underlyingType]
                .Select(f => f(obj))
                .ToList()
                .AsReadOnly();

            // create the Record
            var propertyList = new Record
            {
                UnderlyingType = underlyingType,
                Names = AllPropertyNames[underlyingType],
                Types = AllPropertyTypes[underlyingType],
                Values = values
            };

            return propertyList;
        }
    }
}