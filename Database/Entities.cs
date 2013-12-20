using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using SmartRoutes.Model.Sorta;
using SmartRoutes.Model.Srds;

namespace SmartRoutes.Database
{
    public class Entities : DbContext
    {
        // Destination entities
        public IDbSet<AttributeKey> AttributeKeys { get; set; }
        public IDbSet<AttributeValue> AttributeValues { get; set; }
        public IDbSet<Destination> Destinations { get; set; }

        // GTFS entities
        public IDbSet<Archive> Archives { get; set; }
        public IDbSet<Agency> Agencies { get; set; }
        public IDbSet<Service> Services { get; set; }
        public IDbSet<ServiceException> ServiceException { get; set; }
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

            // give all Id columns "<Type Name>Id"
            modelBuilder
                .Properties()
                .Where(p => p.Name == "Id")
                .Configure(c => c.HasColumnName(c.ClrPropertyInfo.DeclaringType.Name + "Id"));

            // some GTFS entities have their IDs pre-defined
            ISet<Type> preDefinedIds = new HashSet<Type>
            {
                typeof (Agency),
                typeof (Service),
                typeof (Route),
                typeof (Shape),
                typeof (Block),
                typeof (Trip),
                typeof (Stop)
            };
            modelBuilder
                .Properties()
                .Where(p => p.Name == "Id" && preDefinedIds.Contains(p.DeclaringType))
                .Configure(c => c.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None));

            // name the table of the singular version of the entity name
            modelBuilder
                .Types()
                .Configure(c => c.ToTable(c.ClrType.Name));

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
                    .ToTable("CloseStop")
                    .MapLeftKey("StopId")
                    .MapRightKey("CloseStopId"));
        }
    }
}