using System.Data.Entity.Migrations;

namespace Database.Migrations.Odjfs
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