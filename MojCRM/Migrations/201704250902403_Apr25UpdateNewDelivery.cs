namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Apr25UpdateNewDelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deliveries", "FirstInvoice", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Deliveries", "FirstInvoice");
        }
    }
}
