using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations.Odjfs
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.County",
                c => new
                {
                    CountyId = c.Int(false, true),
                    Name = c.String(false, 10),
                    LastScrapedOn = c.DateTime(),
                })
                .PrimaryKey(t => t.CountyId)
                .Index(t => t.Name, true);

            CreateTable(
                "dbo.ChildCare",
                c => new
                {
                    ChildCareId = c.Int(false, true),
                    ChildCareType = c.String(),
                    LastScrapedOn = c.DateTime(false),
                    Latitude = c.Double(),
                    Longitude = c.Double(),
                    CountyId = c.Int(false),
                    ExternalUrlId = c.String(false, 18),
                    ExternalId = c.String(),
                    Name = c.String(),
                    Address = c.String(),
                    City = c.String(),
                    State = c.String(),
                    ZipCode = c.Int(false),
                    PhoneNumber = c.String(),
                })
                .PrimaryKey(t => t.ChildCareId)
                .ForeignKey("dbo.County", t => t.CountyId, true)
                .Index(t => t.CountyId)
                .Index(t => t.ExternalUrlId, true);

            CreateTable(
                "dbo.ChildCareStub",
                c => new
                {
                    ChildCareStubId = c.Int(false, true),
                    CountyId = c.Int(),
                    LastScrapedOn = c.DateTime(),
                    ExternalUrlId = c.String(false, 18),
                    Name = c.String(),
                    Address = c.String(),
                    City = c.String(),
                    ChildCareType = c.String(false, 128),
                })
                .PrimaryKey(t => t.ChildCareStubId)
                .ForeignKey("dbo.County", t => t.CountyId)
                .Index(t => t.CountyId)
                .Index(t => t.ExternalUrlId, true);

            CreateTable(
                "dbo.DayCamp",
                c => new
                {
                    ChildCareId = c.Int(false),
                    Owner = c.String(),
                    RegistrationStatus = c.String(),
                    RegistrationBeginDate = c.String(),
                    RegistrationEndDate = c.String(),
                    EffectiveDates = c.String(),
                    ChildCareType = c.String(false, 128),
                })
                .PrimaryKey(t => t.ChildCareId)
                .ForeignKey("dbo.ChildCare", t => t.ChildCareId)
                .Index(t => t.ChildCareId);

            CreateTable(
                "dbo.DetailedChildCare",
                c => new
                {
                    ChildCareId = c.Int(false),
                    ProviderAgreement = c.String(),
                    Administrators = c.String(),
                    CenterStatus = c.String(),
                    InitialApplicationDate = c.String(),
                    LicenseBeginDate = c.String(),
                    LicenseEndDate = c.String(),
                    SutqRating = c.Int(),
                    Infants = c.Boolean(false),
                    YoungToddlers = c.Boolean(false),
                    OlderToddlers = c.Boolean(false),
                    Preschoolers = c.Boolean(false),
                    Gradeschoolers = c.Boolean(false),
                    ChildCareFoodProgram = c.Boolean(false),
                    Naeyc = c.Boolean(false),
                    Necpa = c.Boolean(false),
                    Naccp = c.Boolean(false),
                    Coa = c.Boolean(false),
                    Acsi = c.Boolean(false),
                    MondayReported = c.Boolean(false),
                    MondayBegin = c.DateTime(),
                    MondayEnd = c.DateTime(),
                    TuesdayReported = c.Boolean(false),
                    TuesdayBegin = c.DateTime(),
                    TuesdayEnd = c.DateTime(),
                    WednesdayReported = c.Boolean(false),
                    WednesdayBegin = c.DateTime(),
                    WednesdayEnd = c.DateTime(),
                    ThursdayReported = c.Boolean(false),
                    ThursdayBegin = c.DateTime(),
                    ThursdayEnd = c.DateTime(),
                    FridayReported = c.Boolean(false),
                    FridayBegin = c.DateTime(),
                    FridayEnd = c.DateTime(),
                    SaturdayReported = c.Boolean(false),
                    SaturdayBegin = c.DateTime(),
                    SaturdayEnd = c.DateTime(),
                    SundayReported = c.Boolean(false),
                    SundayBegin = c.DateTime(),
                    SundayEnd = c.DateTime(),
                    DetailedChildCareType = c.String(false, 128),
                })
                .PrimaryKey(t => t.ChildCareId)
                .ForeignKey("dbo.ChildCare", t => t.ChildCareId)
                .Index(t => t.ChildCareId);

            CreateTable(
                "dbo.TypeBHome",
                c => new
                {
                    ChildCareId = c.Int(false),
                    CertificationBeginDate = c.String(),
                    CertificationEndDate = c.String(),
                    ChildCareType = c.String(false, 128),
                })
                .PrimaryKey(t => t.ChildCareId)
                .ForeignKey("dbo.ChildCare", t => t.ChildCareId)
                .Index(t => t.ChildCareId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.TypeBHome", "ChildCareId", "dbo.ChildCare");
            DropForeignKey("dbo.DetailedChildCare", "ChildCareId", "dbo.ChildCare");
            DropForeignKey("dbo.DayCamp", "ChildCareId", "dbo.ChildCare");
            DropForeignKey("dbo.ChildCareStub", "CountyId", "dbo.County");
            DropForeignKey("dbo.ChildCare", "CountyId", "dbo.County");
            DropIndex("dbo.TypeBHome", new[] {"ChildCareId"});
            DropIndex("dbo.DetailedChildCare", new[] {"ChildCareId"});
            DropIndex("dbo.DayCamp", new[] {"ChildCareId"});
            DropIndex("dbo.ChildCareStub", new[] {"CountyId"});
            DropIndex("dbo.ChildCareStub", new[] {"ExternalUrlId"});
            DropIndex("dbo.ChildCare", new[] {"CountyId"});
            DropIndex("dbo.ChildCare", new[] {"ExternalUrlId"});
            DropIndex("dbo.County", new[] {"Name"});
            DropTable("dbo.TypeBHome");
            DropTable("dbo.DetailedChildCare");
            DropTable("dbo.DayCamp");
            DropTable("dbo.ChildCareStub");
            DropTable("dbo.ChildCare");
            DropTable("dbo.County");
        }
    }
}