using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Model.Sorta;

namespace Database.Contexts
{
    public class SortaEntities : BaseContext
    {
        public SortaEntities() : base("Sorta")
        {
        }

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

            // most entities do not need database generated IDs
            ISet<Type> databaseGeneratedIds = new HashSet<Type>
            {
                typeof (Archive),
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
                .Configure(c => c.ToTable(GetTableName(c.ClrType)));
        }
    }
}