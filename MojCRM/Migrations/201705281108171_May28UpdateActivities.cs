namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class May28UpdateActivities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityLogs", "ReferenceId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityLogs", "ReferenceId");
        }
    }
}
