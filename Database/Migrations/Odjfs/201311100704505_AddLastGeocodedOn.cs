using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations.Odjfs
{
    public partial class AddLastGeocodedOn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChildCare", "LastGeocodedOn", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.ChildCare", "LastGeocodedOn");
        }
    }
}