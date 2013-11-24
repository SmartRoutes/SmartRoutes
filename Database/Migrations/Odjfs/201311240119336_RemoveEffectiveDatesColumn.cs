using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations.Odjfs
{
    public partial class RemoveEffectiveDatesColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DayCamp", "EffectiveDates");
        }

        public override void Down()
        {
            AddColumn("dbo.DayCamp", "EffectiveDates", c => c.String());
        }
    }
}