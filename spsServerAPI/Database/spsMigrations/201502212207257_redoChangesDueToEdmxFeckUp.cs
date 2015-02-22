namespace spsServerAPI.Database.spsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class redoChangesDueToEdmxFeckUp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Placed", "PlacedPlacement_PlacementID", "dbo.Placement");
            DropForeignKey("dbo.Placed", "SID", "dbo.Student");
            DropIndex("dbo.Placed", new[] { "SID" });
            DropIndex("dbo.Placed", new[] { "PlacedPlacement_PlacementID" });
            DropTable("dbo.Placed");
        }
        
        public override void Down()
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
                .PrimaryKey(t => new { t.PID, t.SID });
            
            CreateIndex("dbo.Placed", "PlacedPlacement_PlacementID");
            CreateIndex("dbo.Placed", "SID");
            AddForeignKey("dbo.Placed", "SID", "dbo.Student", "SID", cascadeDelete: true);
            AddForeignKey("dbo.Placed", "PlacedPlacement_PlacementID", "dbo.Placement", "PlacementID");
        }
    }
}
