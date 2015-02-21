namespace spsServerAPI.Database.spsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tutorEmailAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutor", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tutor", "Email");
        }
    }
}
