namespace spsServerAPI.Database.spsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReInitialMigration : DbMigration
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
                        ProviderID = c.Int(nullable: false),
                        AssignedStudentID = c.String(maxLength: 50),
                        SupervisorID = c.Int(),
                        AssignedTutorID = c.Int(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        PlacementDescription = c.String(),
                        PlacementTypeID = c.Int(nullable: false),
                        StartDate = c.DateTime(storeType: "date"),
                        FinishDate = c.DateTime(storeType: "date"),
                        County = c.String(),
                        Country = c.String(),
                        Exclusive = c.Boolean(nullable: false),
                        UseBaseAddress = c.Boolean(),
                    })
                .PrimaryKey(t => t.PlacementID)
                .ForeignKey("dbo.Student", t => t.AssignedStudentID)
                .ForeignKey("dbo.PlacementProvider", t => t.ProviderID, cascadeDelete: true)
                .ForeignKey("dbo.PlacementSupervisor", t => t.SupervisorID)
                .ForeignKey("dbo.Tutor", t => t.AssignedTutorID)
                .ForeignKey("dbo.PlacementType", t => t.PlacementTypeID, cascadeDelete: true)
                .Index(t => t.ProviderID)
                .Index(t => t.AssignedStudentID)
                .Index(t => t.SupervisorID)
                .Index(t => t.AssignedTutorID)
                .Index(t => t.PlacementTypeID);
            
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
                "dbo.StudentPreference",
                c => new
                    {
                        PID = c.Int(nullable: false),
                        SID = c.String(nullable: false, maxLength: 50),
                        Preference = c.Int(nullable: false),
                        Status = c.Int(),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.PID, t.SID })
                .ForeignKey("dbo.Placement", t => t.PID, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.SID, cascadeDelete: true)
                .Index(t => t.PID)
                .Index(t => t.SID);
            
            CreateTable(
                "dbo.StudentProgrammeStage",
                c => new
                    {
                        MemberID = c.Int(nullable: false, identity: true),
                        SID = c.String(maxLength: 50),
                        ProgrammeStageID = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MemberID)
                .ForeignKey("dbo.ProgrammeStage", t => t.ProgrammeStageID, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.SID)
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
                .ForeignKey("dbo.Programme", t => t.ProgrammeCode)
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
                        WebLink = c.String(),
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
                "dbo.Tutor",
                c => new
                    {
                        TutorID = c.Int(nullable: false, identity: true),
                        Email = c.String(),
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
                        TutorID = c.Int(nullable: false),
                        PlacementID = c.Int(nullable: false),
                        DateVisited = c.DateTime(nullable: false, storeType: "date"),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.VisitID)
                .ForeignKey("dbo.Placement", t => t.PlacementID, cascadeDelete: true)
                .ForeignKey("dbo.Tutor", t => t.TutorID, cascadeDelete: true)
                .Index(t => t.TutorID)
                .Index(t => t.PlacementID);
            
            CreateTable(
                "dbo.PlacementType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AllowablePlacement", "ProgrammeStageID", "dbo.ProgrammeStage");
            DropForeignKey("dbo.AllowablePlacement", "PlacementID", "dbo.Placement");
            DropForeignKey("dbo.Placement", "PlacementTypeID", "dbo.PlacementType");
            DropForeignKey("dbo.Placement", "AssignedTutorID", "dbo.Tutor");
            DropForeignKey("dbo.TutorVisits", "TutorID", "dbo.Tutor");
            DropForeignKey("dbo.TutorVisits", "PlacementID", "dbo.Placement");
            DropForeignKey("dbo.Placement", "SupervisorID", "dbo.PlacementSupervisor");
            DropForeignKey("dbo.PlacementSupervisor", "ProviderID", "dbo.PlacementProvider");
            DropForeignKey("dbo.Placement", "ProviderID", "dbo.PlacementProvider");
            DropForeignKey("dbo.Placement", "AssignedStudentID", "dbo.Student");
            DropForeignKey("dbo.StudentProgrammeStage", "SID", "dbo.Student");
            DropForeignKey("dbo.StudentProgrammeStage", "ProgrammeStageID", "dbo.ProgrammeStage");
            DropForeignKey("dbo.ProgrammeStage", "ProgrammeCode", "dbo.Programme");
            DropForeignKey("dbo.StudentPreference", "SID", "dbo.Student");
            DropForeignKey("dbo.StudentPreference", "PID", "dbo.Placement");
            DropIndex("dbo.TutorVisits", new[] { "PlacementID" });
            DropIndex("dbo.TutorVisits", new[] { "TutorID" });
            DropIndex("dbo.PlacementSupervisor", new[] { "ProviderID" });
            DropIndex("dbo.ProgrammeStage", new[] { "ProgrammeCode" });
            DropIndex("dbo.StudentProgrammeStage", new[] { "ProgrammeStageID" });
            DropIndex("dbo.StudentProgrammeStage", new[] { "SID" });
            DropIndex("dbo.StudentPreference", new[] { "SID" });
            DropIndex("dbo.StudentPreference", new[] { "PID" });
            DropIndex("dbo.Placement", new[] { "PlacementTypeID" });
            DropIndex("dbo.Placement", new[] { "AssignedTutorID" });
            DropIndex("dbo.Placement", new[] { "SupervisorID" });
            DropIndex("dbo.Placement", new[] { "AssignedStudentID" });
            DropIndex("dbo.Placement", new[] { "ProviderID" });
            DropIndex("dbo.AllowablePlacement", new[] { "ProgrammeStageID" });
            DropIndex("dbo.AllowablePlacement", new[] { "PlacementID" });
            DropTable("dbo.PlacementType");
            DropTable("dbo.TutorVisits");
            DropTable("dbo.Tutor");
            DropTable("dbo.PlacementSupervisor");
            DropTable("dbo.PlacementProvider");
            DropTable("dbo.Programme");
            DropTable("dbo.ProgrammeStage");
            DropTable("dbo.StudentProgrammeStage");
            DropTable("dbo.StudentPreference");
            DropTable("dbo.Student");
            DropTable("dbo.Placement");
            DropTable("dbo.AllowablePlacement");
        }
    }
}
