namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb3Update : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Organizations");
            AddColumn("dbo.Deliveries", "ReceiverVAT", c => c.String());
            AddColumn("dbo.Deliveries", "ReceiverName", c => c.String());
            AddColumn("dbo.Deliveries", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Deliveries", "Organization_MerId", c => c.Int());
            AlterColumn("dbo.Organizations", "MerId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Organizations", "MerId");
            CreateIndex("dbo.Deliveries", "Organization_MerId");
            AddForeignKey("dbo.Deliveries", "Organization_MerId", "dbo.Organizations", "MerId");
            DropColumn("dbo.Organizations", "OrganizationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Organizations", "OrganizationId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Deliveries", "Organization_MerId", "dbo.Organizations");
            DropIndex("dbo.Deliveries", new[] { "Organization_MerId" });
            DropPrimaryKey("dbo.Organizations");
            AlterColumn("dbo.Organizations", "MerId", c => c.Int(nullable: false));
            DropColumn("dbo.Deliveries", "Organization_MerId");
            DropColumn("dbo.Deliveries", "Discriminator");
            DropColumn("dbo.Deliveries", "ReceiverName");
            DropColumn("dbo.Deliveries", "ReceiverVAT");
            AddPrimaryKey("dbo.Organizations", "OrganizationId");
        }
    }
}
