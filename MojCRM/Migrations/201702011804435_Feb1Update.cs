namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb1Update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Deliveries", "Organizations_OrganizationId", "dbo.Organizations");
            DropIndex("dbo.Deliveries", new[] { "Organizations_OrganizationId" });
            AlterColumn("dbo.Deliveries", "ReceiverId", c => c.Int());
        }
        
        public override void Down()
        {
            AddColumn("dbo.Deliveries", "Organizations_OrganizationId", c => c.Int());
            AlterColumn("dbo.Deliveries", "ReceiverId", c => c.Int(nullable: false));
            CreateIndex("dbo.Deliveries", "Organizations_OrganizationId");
            AddForeignKey("dbo.Deliveries", "Organizations_OrganizationId", "dbo.Organizations", "OrganizationId");
        }
    }
}
