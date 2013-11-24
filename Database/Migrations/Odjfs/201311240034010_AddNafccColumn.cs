using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations.Odjfs
{
    public partial class AddNafccColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetailedChildCare", "Nafcc", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.DetailedChildCare", "Nafcc");
        }
    }
}