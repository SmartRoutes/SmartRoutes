using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using SmartRoutes.Database.Data;

namespace SmartRoutes.Database
{
    public static class ExtensionMethods
    {
        public static IEnumerable<PropertyInfo> GetEntitySetProperties(this DbContext dbContext)
        {
            return dbContext
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly)
                .Where(p => typeof (IDbSet<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition()));
        }

        public static IEnumerable<Type> GetEntityTypes(this DbContext dbContext)
        {
            return dbContext
                .GetEntitySetProperties()
                .Select(p => p.PropertyType.GetGenericArguments().First());
        }

        public static IEnumerable<string> GetTableNames(this DbContext dbContext)
        {
            ObjectContext ctx = ((IObjectContextAdapter) dbContext).ObjectContext;
            MetadataWorkspace mw = ctx.MetadataWorkspace;

            return mw
                .GetItemCollection(DataSpace.SSpace)
                .GetItems<EntityContainer>()
                .SelectMany(e => e.BaseEntitySets)
                .OfType<EntitySet>()
                .Select(s => string.Format("{0}.{1}", s.Schema, s.Table))
                .Distinct()
                .ToArray();
        }
    }
}