using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations.Odjfs
{
    public partial class RenameEndDateColumns : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.TypeBHome", "CertificationEndDate", "CertificationExpirationDate");
            RenameColumn("dbo.DetailedChildCare", "LicenseEndDate", "LicenseExpirationDate");
        }

        public override void Down()
        {
            RenameColumn("dbo.TypeBHome", "CertificationExpirationDate", "CertificationEndDate");
            RenameColumn("dbo.DetailedChildCare", "LicenseExpirationDate", "LicenseEndDate");
        }
    }
}