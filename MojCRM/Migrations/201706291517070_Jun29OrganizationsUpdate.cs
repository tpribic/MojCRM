namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jun29OrganizationsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "FirstReceived", c => c.DateTime());
            AddColumn("dbo.Organizations", "Employees", c => c.Int());
            AddColumn("dbo.Organizations", "Income", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.MerDeliveryDetails", "TotalReceived", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MerDeliveryDetails", "TotalReceived");
            DropColumn("dbo.Organizations", "Income");
            DropColumn("dbo.Organizations", "Employees");
            DropColumn("dbo.Organizations", "FirstReceived");
        }
    }
}
