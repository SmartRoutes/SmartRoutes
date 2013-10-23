using System.Data.Entity;
using Model.Odjfs;

namespace Database.Contexts
{
    public class OdjfsContext : BaseContext
    {
        public OdjfsContext() : base("odjfs")
        {
        }

        public DbSet<ChildCareStub> ChildCareStubs { get; set; }
        public DbSet<TypeAHomeStub> TypeAHomeStubs { get; set; }
        public DbSet<TypeBHomeStub> TypeBHomeStubs { get; set; }
        public DbSet<LicensedCenterStub> LicensedCenterStubs { get; set; }
        public DbSet<DayCampStub> DayCampStubs { get; set; }

        public DbSet<ChildCare> ChildCares { get; set; }
        public DbSet<DetailedChildCare> DetailedChildCares { get; set; }
        public DbSet<TypeAHome> TypeAHomes { get; set; }
        public DbSet<TypeBHome> TypeBHomes { get; set; }
        public DbSet<LicensedCenter> LicensedCenters { get; set; }
        public DbSet<DayCamp> DayCamps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // put all of the tables in the same table schema ("odjfs.<table name>")
            modelBuilder.Types()
                .Where(t => !typeof (ChildCareStub).IsAssignableFrom(t) || t == typeof (ChildCareStub))
                .Where(t => !typeof (DetailedChildCare).IsAssignableFrom(t) || t == typeof (DetailedChildCare))
                .Configure(c => c.ToTable(GetTableName(c.ClrType)));

            // inheritance: table-per-hierarchy
            modelBuilder.Entity<ChildCareStub>()
                .Ignore(e => e.ChildCareType)
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
                .Ignore(e => e.ChildCareType)
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
        }
    }
}