using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model.Odjfs;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;

namespace Database.Contexts
{
    public class OdjfsEntities : BaseContext
    {
        private IDictionary<string, County> _attachedCounties;

        static OdjfsEntities()
        {
            System.Data.Entity.Database.SetInitializer(new Initializer());
        }

        public OdjfsEntities() : base("Odjfs")
        {
        }

        protected override bool ShouldTruncate(string tableName)
        {
            // the County table should should never be truncated
            return tableName != GetTableName(typeof (County));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // put all of the tables in the same table schema ("odjfs.<table name>")
            modelBuilder.Types()
                .Where(t => !typeof (ChildCareStub).IsAssignableFrom(t) || t == typeof (ChildCareStub))
                .Where(t => !typeof (DetailedChildCare).IsAssignableFrom(t) || t == typeof (DetailedChildCare))
                .Configure(c => c.ToTable(GetTableName(c.ClrType)));

            // set up some columns to have unique constraints
            modelBuilder.Entity<ChildCare>()
                .Property(c => c.ExternalUrlId).IsRequired().HasMaxLength(18);

            modelBuilder.Entity<ChildCareStub>()
                .Property(c => c.ExternalUrlId).IsRequired().HasMaxLength(18);

            modelBuilder.Entity<County>()
                .Property(c => c.Name).IsRequired().HasMaxLength(10);

            // inheritance: table-per-hierarchy
            modelBuilder.Entity<ChildCareStub>()
                .Map<TypeAHomeStub>(x => x.Requires("ChildCareType").HasValue(TypeAHomeStub.Discriminator))
                .Map<TypeBHomeStub>(x => x.Requires("ChildCareType").HasValue(TypeBHomeStub.Discriminator))
                .Map<LicensedCenterStub>(x => x.Requires("ChildCareType").HasValue(LicensedCenterStub.Discriminator))
                .Map<DayCampStub>(x => x.Requires("ChildCareType").HasValue(DayCampStub.Discriminator));

            // inheritance: table-per-hierarchy
            modelBuilder.Entity<DetailedChildCare>()
                .Ignore(e => e.DetailedChildCareType)
                .Map<TypeAHome>(x => x.Requires("DetailedChildCareType").HasValue(TypeAHome.DetailedDiscriminator))
                .Map<LicensedCenter>(x => x.Requires("DetailedChildCareType").HasValue(LicensedCenter.DetailedDiscriminator));

            // inheritance: table-per-type
            modelBuilder.Entity<ChildCare>()
                .Map<TypeBHome>(x => x.Requires("ChildCareType").HasValue(TypeBHome.Discriminator))
                .Map<DayCamp>(x => x.Requires("ChildCareType").HasValue(DayCamp.Discriminator))
                .Map<DetailedChildCare>(x => x.Requires("ChildCareType").HasValue(DetailedChildCare.Discriminator));

            // name the ID name columns unique per hierarchy
            modelBuilder
                .Properties()
                .Where(p => p.Name == "Id")
                .Where(p => typeof (ChildCareStub).IsAssignableFrom(p.DeclaringType))
                .Configure(c => c.HasColumnName("ChildCareStubId"));

            modelBuilder
                .Properties()
                .Where(p => p.Name == "Id")
                .Where(p => typeof (ChildCare).IsAssignableFrom(p.DeclaringType))
                .Configure(c => c.HasColumnName("ChildCareId"));

            modelBuilder.Entity<County>()
                .Property(c => c.Id).HasColumnName("CountyId");
        }

        public County GetAttachedCounty(string name)
        {
            if (_attachedCounties == null)
            {
                // create a dictionary of counties, keyed on their name
                _attachedCounties = Counties.ToDictionary(c => c.Name);
            }

            return _attachedCounties[name];
        }

        #region Dependent Entites

        public IDbSet<County> Counties { get; set; }

        #endregion

        #region ChildCareStubs

        public IDbSet<ChildCareStub> ChildCareStubs { get; set; }
        public IDbSet<TypeAHomeStub> TypeAHomeStubs { get; set; }
        public IDbSet<TypeBHomeStub> TypeBHomeStubs { get; set; }
        public IDbSet<LicensedCenterStub> LicensedCenterStubs { get; set; }
        public IDbSet<DayCampStub> DayCampStubs { get; set; }

        #endregion

        #region ChildCares

        public IDbSet<ChildCare> ChildCares { get; set; }
        public IDbSet<DetailedChildCare> DetailedChildCares { get; set; }
        public IDbSet<TypeAHome> TypeAHomes { get; set; }
        public IDbSet<TypeBHome> TypeBHomes { get; set; }
        public IDbSet<LicensedCenter> LicensedCenters { get; set; }
        public IDbSet<DayCamp> DayCamps { get; set; }

        #endregion

        private class Initializer : IDatabaseInitializer<OdjfsEntities>
        {
            private static readonly string[] CountyNames =
            {
                "ADAMS", "ALLEN", "ASHLAND", "ASHTABULA", "ATHENS", "AUGLAIZE", "BELMONT", "BROWN",
                "BUTLER", "CARROLL", "CHAMPAIGN", "CLARK", "CLERMONT", "CLINTON", "COLUMBIANA", "COSHOCTON",
                "CRAWFORD", "CUYAHOGA", "DARKE", "DEFIANCE", "DELAWARE", "ERIE", "FAIRFIELD", "FAYETTE",
                "FRANKLIN", "FULTON", "GALLIA", "GEAUGA", "GREENE", "GUERNSEY", "HAMILTON", "HANCOCK",
                "HARDIN", "HARRISON", "HENRY", "HIGHLAND", "HOCKING", "HOLMES", "HURON", "JACKSON",
                "JEFFERSON", "KNOX", "LAKE", "LAWRENCE", "LICKING", "LOGAN", "LORAIN", "LUCAS",
                "MADISON", "MAHONING", "MARION", "MEDINA", "MEIGS", "MERCER", "MIAMI", "MONROE",
                "MONTGOMERY", "MORGAN", "MORROW", "MUSKINGUM", "NOBLE", "OTTAWA", "PAULDING", "PERRY",
                "PICKAWAY", "PIKE", "PORTAGE", "PREBLE", "PUTNAM", "RICHLAND", "ROSS", "SANDUSKY",
                "SCIOTO", "SENECA", "SHELBY", "STARK", "SUMMIT", "TRUMBULL", "TUSCARAWAS", "UNION",
                "VAN WERT", "VINTON", "WARREN", "WASHINGTON", "WAYNE", "WILLIAMS", "WOOD", "WYANDOT"
            };

            public void InitializeDatabase(OdjfsEntities context)
            {
                if (!context.Database.Exists())
                {
                    context.Database.Create();

                    AddUniqueConstraint(context, typeof (ChildCareStub), "ExternalUrlId");
                    AddUniqueConstraint(context, typeof (ChildCare), "ExternalUrlId");
                    AddUniqueConstraint(context, typeof (County), "Name");

                    context.Database.ExecuteSqlCommand(string.Format(
                        "INSERT INTO {0} (Name) VALUES {1}",
                        context.GetTableName(typeof (County)),
                        string.Join(", ", CountyNames.Select(c => string.Format("('{0}')", c)))));
                }
            }

            private void AddUniqueConstraint(OdjfsEntities context, Type entityType, string propertyName)
            {
                context.Database.ExecuteSqlCommand(string.Format("ALTER TABLE {0} ADD CONSTRAINT [UK_{0}_{1}] UNIQUE ({1})", context.GetTableName(entityType), propertyName));
            }
        }
    }
}