using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartRoutes.Database.Data
{
    public class RecordDataReaderConfiguration<TEntity> where TEntity : class
    {
        private readonly ISet<string> _ignoredPropertyNames;
        private readonly RecordDataReader<TEntity> _recordDataReader;

        public RecordDataReaderConfiguration(RecordDataReader<TEntity> recordDataReader)
        {
            _recordDataReader = recordDataReader;
            _ignoredPropertyNames = new HashSet<string>();
        }

        internal ISet<string> IgnoredPropertyNames
        {
            get { return new HashSet<string>(_ignoredPropertyNames); }
        }

        public RecordDataReaderConfiguration<TEntity> Ignore<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            _recordDataReader.Configured = false;
            _ignoredPropertyNames.Add(GetPropertyName(propertyExpression));
            return this;
        }

        private string GetPropertyName<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            return GetPropertyInfo(propertyExpression).Name;
        }

        private PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            Type entityType = typeof (TEntity);

            var member = propertyExpression.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyExpression));
            }

            var propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyExpression));
            }

            if (entityType != propertyInfo.ReflectedType && !entityType.IsSubclassOf(propertyInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.", propertyExpression, entityType));
            }

            return propertyInfo;
        }
    }
}