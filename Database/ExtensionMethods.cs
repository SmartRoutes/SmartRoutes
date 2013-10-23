using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Model.Sorta;

namespace Database
{
    public static class ExtensionMethods
    {
        public static IEnumerable<PropertyInfo> GetEntitySetProperties(this DbContext dbContext)
        {
            return dbContext
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType.GetGenericTypeDefinition() == typeof (DbSet<>));
        }

        public static IEnumerable<Type> GetEntityTypes(this DbContext dbContext)
        {
            return dbContext
                .GetEntitySetProperties()
                .Select(p => p.PropertyType.GetGenericArguments().First());
        }

        public static IEnumerable<string> GetTableNames(this DbContext dbContext)
        {
            ObjectContext objectContext = ((IObjectContextAdapter) dbContext).ObjectContext;
            MethodInfo createObjectSet = objectContext
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(m => m.Name == "CreateObjectSet" && m.GetParameters().Length == 0);

            return dbContext
                .GetEntityTypes()
                .Select(localType => createObjectSet.MakeGenericMethod(localType).Invoke(objectContext, null))
                .Select(objectSet => ((dynamic) objectSet).ToTraceString())
                .ToArray()
                .Select(traceString => Regex.Match((string) traceString, @"FROM\s+(?<TableName>.+?)\s+AS"))
                .Select(match => match.Groups["TableName"].Value)
                .Distinct();
        }
    }
}