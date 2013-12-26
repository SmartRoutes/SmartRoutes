using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SmartRoutes.Model;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Database
{
    public class Entities : DbContext
    {
        private const string SrdsSchema = "srds";
        private const string GtfsSchema = "gtfs";
        private const string GenericSchema = "gen";

        private static readonly ISet<Type> SrdsTypes = new HashSet<Type>
        {
            typeof (AttributeKey), typeof (AttributeValue), typeof (Destination)
        };

        private static readonly ISet<Type> GtfsTypes = new HashSet<Type>
        {
            typeof (Agency), typeof (Service), typeof (ServiceException),
            typeof (Route), typeof (Shape), typeof (ShapePoint), typeof (Block),
            typeof (Trip), typeof (Stop), typeof (StopTime)
        };

        private static readonly ISet<Type> GenericTypes = new HashSet<Type>
        {
            typeof (Archive)
        };

        public Entities()
        {
            ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = 180;
        }

        // generic entites
        public IDbSet<Archive> Archives { get; set; }

        // SRDS entities
        public IDbSet<SrdsArchive> SrdsArchives { get; set; }
        public IDbSet<AttributeKey> AttributeKeys { get; set; }
        public IDbSet<AttributeValue> AttributeValues { get; set; }
        public IDbSet<Destination> Destinations { get; set; }

        // GTFS entities
        public IDbSet<GtfsArchive> GtfsArchives { get; set; }
        public IDbSet<Agency> Agencies { get; set; }
        public IDbSet<Service> Services { get; set; }
        public IDbSet<ServiceException> ServiceExceptions { get; set; }
        public IDbSet<Route> Routes { get; set; }
        public IDbSet<Shape> Shapes { get; set; }
        public IDbSet<ShapePoint> ShapePoints { get; set; }
        public IDbSet<Block> Blocks { get; set; }
        public IDbSet<Trip> Trips { get; set; }
        public IDbSet<Stop> Stops { get; set; }
        public IDbSet<StopTime> StopTimes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // all entities have client-defined IDs, except Archive
            modelBuilder
                .Properties()
                .Where(p => p.Name == "Id" && !typeof (Archive).IsAssignableFrom(p.DeclaringType))
                .Configure(c => c.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None));

            // name the table of the singular version of the entity name
            modelBuilder
                .Types()
                .Where(t => !typeof (Archive).IsAssignableFrom(t) || t == typeof (Archive))
                .Configure(c => c.ToTable(GetTableName(c)));

            // map the one-to-many for Stop.ChildStops
            modelBuilder
                .Entity<Stop>()
                .HasKey(s => s.Id)
                .HasOptional(s => s.ParentStop)
                .WithMany(s => s.ChildStops)
                .HasForeignKey(s => s.ParentId);

            // map the unidirectional many-to-many for Stop.CloseStops
            modelBuilder
                .Entity<Stop>()
                .HasKey(s => s.Id)
                .HasMany(s => s.CloseStops)
                .WithMany()
                .Map(c => c
                    .ToTable("CloseStop", GtfsSchema)
                    .MapLeftKey("StopId")
                    .MapRightKey("CloseStopId"));

            // inheritance: table-per-hiearchy
            modelBuilder.Entity<Archive>()
                .Ignore(a => a.ArchiveType)
                .Map<SrdsArchive>(x => x.Requires("ArchiveType").HasValue(SrdsArchive.Discriminator))
                .Map<GtfsArchive>(x => x.Requires("ArchiveType").HasValue(GtfsArchive.Discriminator));
        }


        public static string GetTableName(Type c)
        {
            string schema;
            if (SrdsTypes.Contains(c))
            {
                schema = SrdsSchema;
            }
            else if (GtfsTypes.Contains(c))
            {
                schema = GtfsSchema;
            }
            else if (GenericTypes.Contains(c))
            {
                schema = GenericSchema;
            }
            else
            {
                throw new ArgumentException("The provided type is not part of this DbContext or does not have a table.");
            }
            return string.Format("{0}.{1}", schema, c.Name);
        }

        private static string GetTableName(ConventionTypeConfiguration c)
        {
            return GetTableName(c.ClrType);
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

            string[] tableNames = this
                .GetTableNames()
                .Where(n => n != GetTableName(typeof (Archive)))
                .ToArray();

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