namespace spsServerAPI.Database.spsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlacedTableIntroduced : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Placed",
                c => new
                    {
                        PID = c.Int(nullable: false),
                        SID = c.String(nullable: false, maxLength: 50),
                        Year = c.Int(nullable: false),
                        PlacedPlacement_PlacementID = c.Int(),
                    })
                .PrimaryKey(t => new { t.PID, t.SID })
                .ForeignKey("dbo.Placement", t => t.PlacedPlacement_PlacementID)
                .ForeignKey("dbo.Student", t => t.SID, cascadeDelete: true)
                .Index(t => t.SID)
                .Index(t => t.PlacedPlacement_PlacementID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Placed", "SID", "dbo.Student");
            DropForeignKey("dbo.Placed", "PlacedPlacement_PlacementID", "dbo.Placement");
            DropIndex("dbo.Placed", new[] { "PlacedPlacement_PlacementID" });
            DropIndex("dbo.Placed", new[] { "SID" });
            DropTable("dbo.Placed");
        }
    }
}
