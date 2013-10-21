using System.Data.Entity;
using Model.Odjfs;

namespace Database.Contexts
{
    public class OdjfsContext : DbContext
    {
        public OdjfsContext() : base("StreetSmartz")
        {
        }

        public DbSet<ChildCare> ChildCares { get; set; }
        public DbSet<DetailedChildCare> DetailedChildCares { get; set; }
        public DbSet<TypeAHome> TypeAHomes { get; set; }
        public DbSet<TypeBHome> TypeBHomes { get; set; }
        public DbSet<LicensedCenter> LicensedCenters { get; set; }
        public DbSet<DayCamp> DayCamps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set up table-per-hierarchy
            modelBuilder.Entity<ChildCare>()
                .Map<TypeBHome>(x => x.Requires("ChildCareType").HasValue("TypeBHome"))
                .Map<DayCamp>(x => x.Requires("ChildCareType").HasValue("DayCamp"))
                .Map<DetailedChildCare>(x => x.Requires("ChildCareType").HasValue("DetailedChildCare"));

            modelBuilder.Entity<DetailedChildCare>()
                .Map<TypeAHome>(x => x.Requires("DetailedChildCareType").HasValue("TypeAHome"))
                .Map<LicensedCenter>(x => x.Requires("DetailedChildCareType").HasValue("LicensedCenter"));

            /* modelBuilder.Entity<ChildCare>()
                .Map(c => c.Property(e => e.Id).HasColumnName("ChildCareId")); */

            // put all of the tables in the same table schema ("sorta.<table name>")
            modelBuilder.Types()
                .Configure(c => c.ToTable("odjfs." + c.ClrType.Name));
        }
    }
}