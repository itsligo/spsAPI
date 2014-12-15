namespace spsServerAPI.Database.spsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialspsMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AllowablePlacement",
                c => new
                    {
                        PlacementID = c.Int(nullable: false),
                        ProgrammeStageID = c.Int(nullable: false),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => new { t.PlacementID, t.ProgrammeStageID })
                .ForeignKey("dbo.Placement", t => t.PlacementID, cascadeDelete: true)
                .ForeignKey("dbo.ProgrammeStage", t => t.ProgrammeStageID, cascadeDelete: true)
                .Index(t => t.PlacementID)
                .Index(t => t.ProgrammeStageID);
            
            CreateTable(
                "dbo.Placement",
                c => new
                    {
                        PlacementID = c.Int(nullable: false, identity: true),
                        ProviderID = c.Int(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        PlacementDescription = c.String(),
                        PlacementType = c.Int(),
                        WebLink = c.String(),
                        StartDate = c.DateTime(storeType: "date"),
                        FinishDate = c.DateTime(storeType: "date"),
                        County = c.String(),
                        Country = c.String(),
                        Filled = c.Boolean(),
                        UseBaseAddress = c.Boolean(),
                    })
                .PrimaryKey(t => t.PlacementID)
                .ForeignKey("dbo.PlacementProvider", t => t.ProviderID, cascadeDelete: true)
                .ForeignKey("dbo.PlacementType", t => t.PlacementType)
                .Index(t => t.ProviderID)
                .Index(t => t.PlacementType);
            
            CreateTable(
                "dbo.PlacementProvider",
                c => new
                    {
                        ProviderID = c.Int(nullable: false, identity: true),
                        ProviderName = c.String(),
                        ContactNumber = c.String(),
                        ProviderDescription = c.String(),
                        AdditionalNotes = c.String(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        County = c.String(),
                        Country = c.String(),
                    })
                .PrimaryKey(t => t.ProviderID);
            
            CreateTable(
                "dbo.PlacementSupervisor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProviderID = c.Int(),
                        FirstName = c.String(),
                        SecondName = c.String(),
                        Telephone = c.String(maxLength: 50),
                        email = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlacementProvider", t => t.ProviderID)
                .Index(t => t.ProviderID);
            
            CreateTable(
                "dbo.PlacementType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentPlacement",
                c => new
                    {
                        SPID = c.Int(nullable: false, identity: true),
                        SID = c.String(maxLength: 50),
                        PlacementID = c.Int(),
                        Preference = c.Int(),
                        Status = c.Int(),
                        TutorID = c.Int(),
                        SupervisorID = c.Int(),
                    })
                .PrimaryKey(t => t.SPID)
                .ForeignKey("dbo.Student", t => t.SID, cascadeDelete: true)
                .ForeignKey("dbo.Tutor", t => t.TutorID, cascadeDelete: true)
                .ForeignKey("dbo.Placement", t => t.PlacementID, cascadeDelete: true)
                .Index(t => t.SID)
                .Index(t => t.PlacementID)
                .Index(t => t.TutorID);
            
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        SID = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(),
                        SecondName = c.String(),
                    })
                .PrimaryKey(t => t.SID);
            
            CreateTable(
                "dbo.StudentProgrammeStage",
                c => new
                    {
                        MemberID = c.Int(nullable: false, identity: true),
                        SID = c.String(maxLength: 50),
                        ProgrammeStageID = c.Int(),
                    })
                .PrimaryKey(t => t.MemberID)
                .ForeignKey("dbo.ProgrammeStage", t => t.ProgrammeStageID, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.SID, cascadeDelete: true)
                .Index(t => t.SID)
                .Index(t => t.ProgrammeStageID);
            
            CreateTable(
                "dbo.ProgrammeStage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgrammeCode = c.String(maxLength: 50),
                        Stage = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Programme", t => t.ProgrammeCode, cascadeDelete: true)
                .Index(t => t.ProgrammeCode);
            
            CreateTable(
                "dbo.Programme",
                c => new
                    {
                        ProgrammeCode = c.String(nullable: false, maxLength: 50),
                        ProgrammeName = c.String(),
                    })
                .PrimaryKey(t => t.ProgrammeCode);
            
            CreateTable(
                "dbo.Tutor",
                c => new
                    {
                        TutorID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        SecondName = c.String(),
                        ContactNumber1 = c.String(maxLength: 50),
                        ContactNumber2 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.TutorID);
            
            CreateTable(
                "dbo.TutorVisits",
                c => new
                    {
                        VisitID = c.Int(nullable: false, identity: true),
                        TutorID = c.Int(),
                        SPID = c.Int(),
                        DateVisited = c.DateTime(storeType: "date"),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.VisitID)
                .ForeignKey("dbo.StudentPlacement", t => t.SPID)
                .ForeignKey("dbo.Tutor", t => t.TutorID, cascadeDelete: true)
                .Index(t => t.TutorID)
                .Index(t => t.SPID);
            
            CreateTable(
                "dbo.AvailablePlacementsView",
                c => new
                    {
                        PlacementID = c.Int(nullable: false),
                        Id = c.Int(nullable: false),
                        ProviderID = c.Int(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        PlacementDescription = c.String(),
                        PlacementType = c.Int(),
                        WebLink = c.String(),
                        StartDate = c.DateTime(storeType: "date"),
                        FinishDate = c.DateTime(storeType: "date"),
                        County = c.String(),
                        Country = c.String(),
                        Filled = c.Boolean(),
                        UseBaseAddress = c.Boolean(),
                        ProgrammeCode = c.String(maxLength: 50),
                        Stage = c.Int(),
                        ProgrammeName = c.String(),
                    })
                .PrimaryKey(t => new { t.PlacementID, t.Id });
            
            CreateTable(
                "dbo.PlacementProviderView",
                c => new
                    {
                        ProviderID = c.Int(nullable: false, identity: true),
                        ProviderName = c.String(),
                        ContactNumber = c.String(),
                        ProviderDescription = c.String(),
                        AdditionalNotes = c.String(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        County = c.String(),
                        Country = c.String(),
                    })
                .PrimaryKey(t => t.ProviderID);
            
            CreateTable(
                "dbo.PlacementTypeListView",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlacementView",
                c => new
                    {
                        PlacementID = c.Int(nullable: false),
                        Id = c.Int(nullable: false),
                        ProviderID = c.Int(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        PlacementDescription = c.String(),
                        PlacementType = c.Int(),
                        WebLink = c.String(),
                        StartDate = c.DateTime(storeType: "date"),
                        FinishDate = c.DateTime(storeType: "date"),
                        County = c.String(),
                        Country = c.String(),
                        Filled = c.Boolean(),
                        UseBaseAddress = c.Boolean(),
                        ProgrammeCode = c.String(maxLength: 50),
                        Stage = c.Int(),
                        ProgrammeName = c.String(),
                    })
                .PrimaryKey(t => new { t.PlacementID, t.Id });
            
            CreateTable(
                "dbo.PlacementYearsView",
                c => new
                    {
                        PlacementID = c.Int(nullable: false, identity: true),
                        PlacementYear = c.Int(),
                    })
                .PrimaryKey(t => t.PlacementID);
            
            CreateTable(
                "dbo.ProgrammeListView",
                c => new
                    {
                        ProgrammeCode = c.String(nullable: false, maxLength: 50),
                        ProgrammeName = c.String(),
                    })
                .PrimaryKey(t => t.ProgrammeCode);
            
            CreateTable(
                "dbo.ProgrammeStagesView",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ProgrammeCode = c.String(maxLength: 50),
                        ProgrammeName = c.String(),
                        Stage = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProvidersNameListView",
                c => new
                    {
                        ProviderID = c.Int(nullable: false, identity: true),
                        ProviderName = c.String(),
                    })
                .PrimaryKey(t => t.ProviderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentPlacement", "PlacementID", "dbo.Placement");
            DropForeignKey("dbo.TutorVisits", "TutorID", "dbo.Tutor");
            DropForeignKey("dbo.TutorVisits", "SPID", "dbo.StudentPlacement");
            DropForeignKey("dbo.StudentPlacement", "TutorID", "dbo.Tutor");
            DropForeignKey("dbo.StudentProgrammeStage", "SID", "dbo.Student");
            DropForeignKey("dbo.StudentProgrammeStage", "ProgrammeStageID", "dbo.ProgrammeStage");
            DropForeignKey("dbo.ProgrammeStage", "ProgrammeCode", "dbo.Programme");
            DropForeignKey("dbo.AllowablePlacement", "ProgrammeStageID", "dbo.ProgrammeStage");
            DropForeignKey("dbo.StudentPlacement", "SID", "dbo.Student");
            DropForeignKey("dbo.Placement", "PlacementType", "dbo.PlacementType");
            DropForeignKey("dbo.PlacementSupervisor", "ProviderID", "dbo.PlacementProvider");
            DropForeignKey("dbo.Placement", "ProviderID", "dbo.PlacementProvider");
            DropForeignKey("dbo.AllowablePlacement", "PlacementID", "dbo.Placement");
            DropIndex("dbo.TutorVisits", new[] { "SPID" });
            DropIndex("dbo.TutorVisits", new[] { "TutorID" });
            DropIndex("dbo.ProgrammeStage", new[] { "ProgrammeCode" });
            DropIndex("dbo.StudentProgrammeStage", new[] { "ProgrammeStageID" });
            DropIndex("dbo.StudentProgrammeStage", new[] { "SID" });
            DropIndex("dbo.StudentPlacement", new[] { "TutorID" });
            DropIndex("dbo.StudentPlacement", new[] { "PlacementID" });
            DropIndex("dbo.StudentPlacement", new[] { "SID" });
            DropIndex("dbo.PlacementSupervisor", new[] { "ProviderID" });
            DropIndex("dbo.Placement", new[] { "PlacementType" });
            DropIndex("dbo.Placement", new[] { "ProviderID" });
            DropIndex("dbo.AllowablePlacement", new[] { "ProgrammeStageID" });
            DropIndex("dbo.AllowablePlacement", new[] { "PlacementID" });
            DropTable("dbo.ProvidersNameListView");
            DropTable("dbo.ProgrammeStagesView");
            DropTable("dbo.ProgrammeListView");
            DropTable("dbo.PlacementYearsView");
            DropTable("dbo.PlacementView");
            DropTable("dbo.PlacementTypeListView");
            DropTable("dbo.PlacementProviderView");
            DropTable("dbo.AvailablePlacementsView");
            DropTable("dbo.TutorVisits");
            DropTable("dbo.Tutor");
            DropTable("dbo.Programme");
            DropTable("dbo.ProgrammeStage");
            DropTable("dbo.StudentProgrammeStage");
            DropTable("dbo.Student");
            DropTable("dbo.StudentPlacement");
            DropTable("dbo.PlacementType");
            DropTable("dbo.PlacementSupervisor");
            DropTable("dbo.PlacementProvider");
            DropTable("dbo.Placement");
            DropTable("dbo.AllowablePlacement");
        }
    }
}
