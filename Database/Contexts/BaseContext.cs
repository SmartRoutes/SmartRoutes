using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Contexts
{
    public class BaseContext : DbContext
    {
        private readonly string _schema;

        protected BaseContext(string schema) : base("StreetSmartz")
        {
            _schema = schema;
        }

        protected string GetTableName(Type type)
        {
            return string.Format("{0}.{1}", _schema, type.Name);
        }

        public async Task TruncateAsync()
        {
            foreach (string query in GetTruncateQueries())
            {
                await Database.ExecuteSqlCommandAsync(query);
            }
        }

        public void Truncate()
        {
            foreach (string query in GetTruncateQueries())
            {
                Database.ExecuteSqlCommand(query);
            }
        }

        private IEnumerable<string> GetTruncateQueries()
        {
            if (!Database.Exists())
            {
                yield break;
            }

            string[] queries =
            {
                "ALTER TABLE {0} NOCHECK CONSTRAINT all",
                "DELETE FROM {0}",
                "ALTER TABLE {0} WITH CHECK CHECK CONSTRAINT all",
                "IF EXISTS (SELECT 1 FROM sys.objects o INNER JOIN sys.columns c ON o.object_id = c.object_id WHERE c.is_identity = 1 AND o.[object_id] = object_id('{0}')) DBCC CHECKIDENT ('{0}', RESEED, 0)"
            };

            string[] tableNames = this.GetTableNames().ToArray();

            foreach (string query in queries)
            {
                foreach (string tableName in tableNames)
                {
                    yield return string.Format(query, tableName);
                }
            }
        }
    }
}