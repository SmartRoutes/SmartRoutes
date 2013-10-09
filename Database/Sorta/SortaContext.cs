using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Model.Sorta;

namespace Database.Sorta
{
    public class SortaContext : DbContext
    {
        public SortaContext() : base("StreetSmartz")
        {
        }

        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceException> ServiceException { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Shape> Shapes { get; set; }
        public DbSet<ShapePoint> ShapePoints { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<StopTime> StopTimes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // give all Id columns "<Type Name>Id"
            modelBuilder
                .Properties()
                .Where(p => p.Name == "Id")
                .Configure(c => c.HasColumnName(c.ClrPropertyInfo.DeclaringType.Name + "Id"));

            // most entities do not need database generated IDs
            ISet<Type> databaseGeneratedIds = new HashSet<Type>
                                              {
                                                  typeof (ShapePoint),
                                                  typeof (StopTime),
                                                  typeof (ServiceException)
                                              };
            modelBuilder
                .Properties()
                .Where(p => p.Name == "Id" && !databaseGeneratedIds.Contains(p.DeclaringType))
                .Configure(c => c.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None));

            // put all of the tables in the same table schema ("sorta.<table name>")
            modelBuilder
                .Types()
                .Configure(c => c.ToTable("sorta." + c.ClrType.Name));

            base.OnModelCreating(modelBuilder);
        }
    }
}