using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations
{
    public partial class RemoveClosestStops : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("gtfs.CloseStop", "StopId", "gtfs.Stop");
            DropForeignKey("gtfs.CloseStop", "CloseStopId", "gtfs.Stop");
            DropIndex("gtfs.CloseStop", new[] {"StopId"});
            DropIndex("gtfs.CloseStop", new[] {"CloseStopId"});
            DropTable("gtfs.CloseStop");
        }

        public override void Down()
        {
            CreateTable(
                "gtfs.CloseStop",
                c => new
                {
                    StopId = c.Int(false),
                    CloseStopId = c.Int(false),
                })
                .PrimaryKey(t => new {t.StopId, t.CloseStopId});

            CreateIndex("gtfs.CloseStop", "CloseStopId");
            CreateIndex("gtfs.CloseStop", "StopId");
            AddForeignKey("gtfs.CloseStop", "CloseStopId", "gtfs.Stop", "Id");
            AddForeignKey("gtfs.CloseStop", "StopId", "gtfs.Stop", "Id");
        }
    }
}