namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nov11Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deliveries", "LastGetSentDocumentsDate", c => c.DateTime());
            AddColumn("dbo.Deliveries", "GetSentDocumentsResult", c => c.String());
            AddColumn("dbo.ActivityLogs", "IsSuspiciousActivity", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityLogs", "IsSuspiciousActivity");
            DropColumn("dbo.Deliveries", "GetSentDocumentsResult");
            DropColumn("dbo.Deliveries", "LastGetSentDocumentsDate");
        }
    }
}
