using System.Data.Entity.Migrations;

namespace Database.Migrations.Sorta
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agency",
                c => new
                {
                    AgencyId = c.String(false, 128),
                    Name = c.String(),
                    Url = c.String(),
                    Timezone = c.String(),
                    Language = c.String(),
                    Phone = c.String(),
                    FareUrl = c.String(),
                })
                .PrimaryKey(t => t.AgencyId);

            CreateTable(
                "dbo.Route",
                c => new
                {
                    RouteId = c.Int(false),
                    ShortName = c.String(),
                    Description = c.String(),
                    AgencyId = c.String(maxLength: 128),
                    LongName = c.String(),
                    RouteTypeId = c.Int(false),
                    Url = c.String(),
                    Color = c.String(),
                    TextColor = c.String(),
                })
                .PrimaryKey(t => t.RouteId)
                .ForeignKey("dbo.Agency", t => t.AgencyId)
                .Index(t => t.AgencyId);

            CreateTable(
                "dbo.Trip",
                c => new
                {
                    TripId = c.Int(false),
                    RouteId = c.Int(false),
                    ServiceId = c.Int(false),
                    Headsign = c.String(),
                    ShortName = c.String(),
                    DirectionId = c.Int(),
                    BlockId = c.Int(),
                    ShapeId = c.Int(),
                    WheelchairAccessible = c.Int(),
                })
                .PrimaryKey(t => t.TripId)
                .ForeignKey("dbo.Block", t => t.BlockId)
                .ForeignKey("dbo.Route", t => t.RouteId, true)
                .ForeignKey("dbo.Service", t => t.ServiceId, true)
                .ForeignKey("dbo.Shape", t => t.ShapeId)
                .Index(t => t.BlockId)
                .Index(t => t.RouteId)
                .Index(t => t.ServiceId)
                .Index(t => t.ShapeId);

            CreateTable(
                "dbo.Block",
                c => new
                {
                    BlockId = c.Int(false),
                })
                .PrimaryKey(t => t.BlockId);

            CreateTable(
                "dbo.Service",
                c => new
                {
                    ServiceId = c.Int(false),
                    Monday = c.Boolean(false),
                    Tuesday = c.Boolean(false),
                    Wednesday = c.Boolean(false),
                    Thursday = c.Boolean(false),
                    Friday = c.Boolean(false),
                    Saturday = c.Boolean(false),
                    Sunday = c.Boolean(false),
                    StartDate = c.DateTime(false),
                    EndDate = c.DateTime(false),
                })
                .PrimaryKey(t => t.ServiceId);

            CreateTable(
                "dbo.ServiceException",
                c => new
                {
                    ServiceExceptionId = c.Int(false, true),
                    ServiceId = c.Int(false),
                    Date = c.DateTime(false),
                    ServiceExemptionTypeId = c.Int(false),
                })
                .PrimaryKey(t => t.ServiceExceptionId)
                .ForeignKey("dbo.Service", t => t.ServiceId, true)
                .Index(t => t.ServiceId);

            CreateTable(
                "dbo.Shape",
                c => new
                {
                    ShapeId = c.Int(false),
                })
                .PrimaryKey(t => t.ShapeId);

            CreateTable(
                "dbo.ShapePoint",
                c => new
                {
                    ShapePointId = c.Int(false, true),
                    ShapeId = c.Int(false),
                    Latitude = c.Double(false),
                    Longitude = c.Double(false),
                    Sequence = c.Int(false),
                    DistanceTraveled = c.Double(),
                })
                .PrimaryKey(t => t.ShapePointId)
                .ForeignKey("dbo.Shape", t => t.ShapeId, true)
                .Index(t => t.ShapeId);

            CreateTable(
                "dbo.StopTime",
                c => new
                {
                    StopTimeId = c.Int(false, true),
                    TripId = c.Int(false),
                    ArrivalTime = c.DateTime(false),
                    DepartureTime = c.DateTime(false),
                    StopId = c.Int(false),
                    Sequence = c.Int(false),
                    Headsign = c.String(),
                    PickupType = c.Int(),
                    DropOffType = c.Int(),
                    ShapeDistanceTraveled = c.Double(),
                })
                .PrimaryKey(t => t.StopTimeId)
                .ForeignKey("dbo.Stop", t => t.StopId, true)
                .ForeignKey("dbo.Trip", t => t.TripId, true)
                .Index(t => t.StopId)
                .Index(t => t.TripId);

            CreateTable(
                "dbo.Stop",
                c => new
                {
                    StopId = c.Int(false),
                    Code = c.String(),
                    Name = c.String(),
                    Latitude = c.Double(false),
                    Longitude = c.Double(false),
                    ZoneId = c.Int(),
                    Url = c.String(),
                    TypeId = c.Int(),
                    ParentId = c.Int(),
                    Timezone = c.String(),
                    WheelchairBoarding = c.Int(),
                    ParentStop_Id = c.Int(),
                })
                .PrimaryKey(t => t.StopId)
                .ForeignKey("dbo.Stop", t => t.ParentStop_Id)
                .Index(t => t.ParentStop_Id);

            CreateTable(
                "dbo.Archive",
                c => new
                {
                    ArchiveId = c.Int(false, true),
                    ETag = c.String(),
                    Hash = c.String(),
                    DownloadedOn = c.DateTime(false),
                })
                .PrimaryKey(t => t.ArchiveId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.StopTime", "TripId", "dbo.Trip");
            DropForeignKey("dbo.StopTime", "StopId", "dbo.Stop");
            DropForeignKey("dbo.Stop", "ParentStop_Id", "dbo.Stop");
            DropForeignKey("dbo.Trip", "ShapeId", "dbo.Shape");
            DropForeignKey("dbo.ShapePoint", "ShapeId", "dbo.Shape");
            DropForeignKey("dbo.Trip", "ServiceId", "dbo.Service");
            DropForeignKey("dbo.ServiceException", "ServiceId", "dbo.Service");
            DropForeignKey("dbo.Trip", "RouteId", "dbo.Route");
            DropForeignKey("dbo.Trip", "BlockId", "dbo.Block");
            DropForeignKey("dbo.Route", "AgencyId", "dbo.Agency");
            DropIndex("dbo.StopTime", new[] {"TripId"});
            DropIndex("dbo.StopTime", new[] {"StopId"});
            DropIndex("dbo.Stop", new[] {"ParentStop_Id"});
            DropIndex("dbo.Trip", new[] {"ShapeId"});
            DropIndex("dbo.ShapePoint", new[] {"ShapeId"});
            DropIndex("dbo.Trip", new[] {"ServiceId"});
            DropIndex("dbo.ServiceException", new[] {"ServiceId"});
            DropIndex("dbo.Trip", new[] {"RouteId"});
            DropIndex("dbo.Trip", new[] {"BlockId"});
            DropIndex("dbo.Route", new[] {"AgencyId"});
            DropTable("dbo.Archive");
            DropTable("dbo.Stop");
            DropTable("dbo.StopTime");
            DropTable("dbo.ShapePoint");
            DropTable("dbo.Shape");
            DropTable("dbo.ServiceException");
            DropTable("dbo.Service");
            DropTable("dbo.Block");
            DropTable("dbo.Trip");
            DropTable("dbo.Route");
            DropTable("dbo.Agency");
        }
    }
}