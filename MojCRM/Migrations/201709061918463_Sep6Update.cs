namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sep6Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AcquireEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedOrganizationId = c.Int(),
                        RelatedCampaignId = c.Int(),
                        IsAssigned = c.Boolean(nullable: false),
                        AssignedTo = c.String(),
                        AcquireEmailStatus = c.Int(nullable: false),
                        LastContactedBy = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        LastContactDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Campaigns", t => t.RelatedCampaignId)
                .ForeignKey("dbo.Organizations", t => t.RelatedOrganizationId)
                .Index(t => t.RelatedOrganizationId)
                .Index(t => t.RelatedCampaignId);
            
            AddColumn("dbo.MerDeliveryDetails", "AcquiredReceivingInformation", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AcquireEmails", "RelatedOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.AcquireEmails", "RelatedCampaignId", "dbo.Campaigns");
            DropIndex("dbo.AcquireEmails", new[] { "RelatedCampaignId" });
            DropIndex("dbo.AcquireEmails", new[] { "RelatedOrganizationId" });
            DropColumn("dbo.MerDeliveryDetails", "AcquiredReceivingInformation");
            DropTable("dbo.AcquireEmails");
        }
    }
}
