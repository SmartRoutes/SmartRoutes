using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations.Sorta
{
    public partial class FixParentStop : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stop", "ParentStop_Id", "dbo.Stop");
            DropIndex("dbo.Stop", new[] {"ParentStop_Id"});
            CreateIndex("dbo.Stop", "ParentId");
            AddForeignKey("dbo.Stop", "ParentId", "dbo.Stop", "StopId");
            DropColumn("dbo.Stop", "ParentStop_Id");
        }

        public override void Down()
        {
            AddColumn("dbo.Stop", "ParentStop_Id", c => c.Int());
            DropForeignKey("dbo.Stop", "ParentId", "dbo.Stop");
            DropIndex("dbo.Stop", new[] {"ParentId"});
            CreateIndex("dbo.Stop", "ParentStop_Id");
            AddForeignKey("dbo.Stop", "ParentStop_Id", "dbo.Stop", "StopId");
        }
    }
}