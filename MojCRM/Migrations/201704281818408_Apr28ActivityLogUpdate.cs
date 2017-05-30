namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Apr28ActivityLogUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityLogs", "Department", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityLogs", "Department");
        }
    }
}
