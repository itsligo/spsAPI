namespace spsServerAPI.Database.spsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeProgrammeStageYearToInt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StudentProgrammeStage", "Year");
            AddColumn("dbo.StudentProgrammeStage", "Year", c => c.Int());
        }
        
    }
}
