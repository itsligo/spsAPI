namespace spsServerAPI.Database.spsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProgrammeStageYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudentProgrammeStage", "Year", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudentProgrammeStage", "Year");
        }
    }
}
