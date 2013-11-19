using System.Data.Entity.Migrations;

namespace Database.Migrations.Sorta
{
    public partial class AddCloseStops : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CloseStop",
                c => new
                {
                    StopId = c.Int(false),
                    CloseStopId = c.Int(false),
                })
                .PrimaryKey(t => new {t.StopId, t.CloseStopId})
                .ForeignKey("dbo.Stop", t => t.StopId)
                .ForeignKey("dbo.Stop", t => t.CloseStopId)
                .Index(t => t.StopId)
                .Index(t => t.CloseStopId);

            // after this migration, we need to re-parse
            Sql("DELETE FROM dbo.Archive");
        }

        public override void Down()
        {
            DropForeignKey("dbo.CloseStop", "CloseStopId", "dbo.Stop");
            DropForeignKey("dbo.CloseStop", "StopId", "dbo.Stop");
            DropIndex("dbo.CloseStop", new[] {"CloseStopId"});
            DropIndex("dbo.CloseStop", new[] {"StopId"});
            DropTable("dbo.CloseStop");

            // after this migration, we need to re-parse
            Sql("DELETE FROM dbo.Archive");
        }
    }
}