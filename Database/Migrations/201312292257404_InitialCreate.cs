using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "gtfs.Agency",
                c => new
                {
                    Id = c.String(false, 128),
                    Name = c.String(),
                    Url = c.String(),
                    Timezone = c.String(),
                    Language = c.String(),
                    Phone = c.String(),
                    FareUrl = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gtfs.Route",
                c => new
                {
                    Id = c.Int(false),
                    ShortName = c.String(),
                    Description = c.String(),
                    AgencyId = c.String(maxLength: 128),
                    LongName = c.String(),
                    RouteTypeId = c.Int(false),
                    Url = c.String(),
                    Color = c.String(),
                    TextColor = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gtfs.Agency", t => t.AgencyId)
                .Index(t => t.AgencyId);

            CreateTable(
                "gtfs.Trip",
                c => new
                {
                    Id = c.Int(false),
                    RouteId = c.Int(false),
                    ServiceId = c.Int(false),
                    Headsign = c.String(),
                    ShortName = c.String(),
                    DirectionId = c.Int(),
                    BlockId = c.Int(),
                    ShapeId = c.Int(),
                    WheelchairAccessible = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gtfs.Block", t => t.BlockId)
                .ForeignKey("gtfs.Route", t => t.RouteId, true)
                .ForeignKey("gtfs.Service", t => t.ServiceId, true)
                .ForeignKey("gtfs.Shape", t => t.ShapeId)
                .Index(t => t.BlockId)
                .Index(t => t.RouteId)
                .Index(t => t.ServiceId)
                .Index(t => t.ShapeId);

            CreateTable(
                "gtfs.Block",
                c => new
                {
                    Id = c.Int(false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gtfs.Service",
                c => new
                {
                    Id = c.Int(false),
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
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gtfs.ServiceException",
                c => new
                {
                    Id = c.Int(false),
                    ServiceId = c.Int(false),
                    Date = c.DateTime(false),
                    ServiceExemptionTypeId = c.Int(false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gtfs.Service", t => t.ServiceId, true)
                .Index(t => t.ServiceId);

            CreateTable(
                "gtfs.Shape",
                c => new
                {
                    Id = c.Int(false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gtfs.ShapePoint",
                c => new
                {
                    Id = c.Int(false),
                    ShapeId = c.Int(false),
                    Latitude = c.Double(false),
                    Longitude = c.Double(false),
                    Sequence = c.Int(false),
                    DistanceTraveled = c.Double(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gtfs.Shape", t => t.ShapeId, true)
                .Index(t => t.ShapeId);

            CreateTable(
                "gtfs.StopTime",
                c => new
                {
                    Id = c.Int(false),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("gtfs.Stop", t => t.StopId, true)
                .ForeignKey("gtfs.Trip", t => t.TripId, true)
                .Index(t => t.StopId)
                .Index(t => t.TripId);

            CreateTable(
                "gtfs.Stop",
                c => new
                {
                    Id = c.Int(false),
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
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gtfs.Stop", t => t.ParentId)
                .Index(t => t.ParentId);

            CreateTable(
                "gen.Archive",
                c => new
                {
                    Id = c.Int(false, true),
                    Hash = c.String(),
                    LoadedOn = c.DateTime(false),
                    ArchiveType = c.String(false, 128),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "srds.AttributeKey",
                c => new
                {
                    Id = c.Int(false),
                    Name = c.String(),
                    TypeName = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "srds.AttributeValue",
                c => new
                {
                    Id = c.Int(false),
                    AttributeKeyId = c.Int(false),
                    DestinationId = c.Int(false),
                    ValueString = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("srds.AttributeKey", t => t.AttributeKeyId, true)
                .ForeignKey("srds.Destination", t => t.DestinationId, true)
                .Index(t => t.AttributeKeyId)
                .Index(t => t.DestinationId);

            CreateTable(
                "srds.Destination",
                c => new
                {
                    Id = c.Int(false),
                    Name = c.String(),
                    Latitude = c.Double(false),
                    Longitude = c.Double(false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "gtfs.CloseStop",
                c => new
                {
                    StopId = c.Int(false),
                    CloseStopId = c.Int(false),
                })
                .PrimaryKey(t => new {t.StopId, t.CloseStopId})
                .ForeignKey("gtfs.Stop", t => t.StopId)
                .ForeignKey("gtfs.Stop", t => t.CloseStopId)
                .Index(t => t.StopId)
                .Index(t => t.CloseStopId);
        }

        public override void Down()
        {
            DropForeignKey("srds.AttributeValue", "DestinationId", "srds.Destination");
            DropForeignKey("srds.AttributeValue", "AttributeKeyId", "srds.AttributeKey");
            DropForeignKey("gtfs.StopTime", "TripId", "gtfs.Trip");
            DropForeignKey("gtfs.StopTime", "StopId", "gtfs.Stop");
            DropForeignKey("gtfs.Stop", "ParentId", "gtfs.Stop");
            DropForeignKey("gtfs.CloseStop", "CloseStopId", "gtfs.Stop");
            DropForeignKey("gtfs.CloseStop", "StopId", "gtfs.Stop");
            DropForeignKey("gtfs.Trip", "ShapeId", "gtfs.Shape");
            DropForeignKey("gtfs.ShapePoint", "ShapeId", "gtfs.Shape");
            DropForeignKey("gtfs.Trip", "ServiceId", "gtfs.Service");
            DropForeignKey("gtfs.ServiceException", "ServiceId", "gtfs.Service");
            DropForeignKey("gtfs.Trip", "RouteId", "gtfs.Route");
            DropForeignKey("gtfs.Trip", "BlockId", "gtfs.Block");
            DropForeignKey("gtfs.Route", "AgencyId", "gtfs.Agency");
            DropIndex("srds.AttributeValue", new[] {"DestinationId"});
            DropIndex("srds.AttributeValue", new[] {"AttributeKeyId"});
            DropIndex("gtfs.StopTime", new[] {"TripId"});
            DropIndex("gtfs.StopTime", new[] {"StopId"});
            DropIndex("gtfs.Stop", new[] {"ParentId"});
            DropIndex("gtfs.CloseStop", new[] {"CloseStopId"});
            DropIndex("gtfs.CloseStop", new[] {"StopId"});
            DropIndex("gtfs.Trip", new[] {"ShapeId"});
            DropIndex("gtfs.ShapePoint", new[] {"ShapeId"});
            DropIndex("gtfs.Trip", new[] {"ServiceId"});
            DropIndex("gtfs.ServiceException", new[] {"ServiceId"});
            DropIndex("gtfs.Trip", new[] {"RouteId"});
            DropIndex("gtfs.Trip", new[] {"BlockId"});
            DropIndex("gtfs.Route", new[] {"AgencyId"});
            DropTable("gtfs.CloseStop");
            DropTable("srds.Destination");
            DropTable("srds.AttributeValue");
            DropTable("srds.AttributeKey");
            DropTable("gen.Archive");
            DropTable("gtfs.Stop");
            DropTable("gtfs.StopTime");
            DropTable("gtfs.ShapePoint");
            DropTable("gtfs.Shape");
            DropTable("gtfs.ServiceException");
            DropTable("gtfs.Service");
            DropTable("gtfs.Block");
            DropTable("gtfs.Trip");
            DropTable("gtfs.Route");
            DropTable("gtfs.Agency");
        }
    }
}