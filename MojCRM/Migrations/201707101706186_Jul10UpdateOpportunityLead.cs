namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul10UpdateOpportunityLead : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Leads",
                c => new
                    {
                        LeadId = c.Int(nullable: false, identity: true),
                        LeadTitle = c.String(),
                        RelatedCampaignId = c.Int(),
                        RelatedOpportunityId = c.Int(),
                        RelatedOrganizationId = c.Int(),
                        LeadStatus = c.Int(nullable: false),
                        RejectReason = c.Int(nullable: false),
                        QuoteType = c.Int(),
                        CreatedBy = c.String(),
                        AssignedTo = c.String(),
                        LastUpdatedBy = c.String(),
                        IsAssigned = c.Boolean(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.LeadId)
                .ForeignKey("dbo.Campaigns", t => t.RelatedCampaignId)
                .ForeignKey("dbo.Opportunities", t => t.RelatedOpportunityId)
                .ForeignKey("dbo.Organizations", t => t.RelatedOrganizationId)
                .Index(t => t.RelatedCampaignId)
                .Index(t => t.RelatedOpportunityId)
                .Index(t => t.RelatedOrganizationId);
            
            CreateTable(
                "dbo.Opportunities",
                c => new
                    {
                        OpportunityId = c.Int(nullable: false, identity: true),
                        OpportunityTitle = c.String(),
                        RelatedCampaignId = c.Int(),
                        RelatedOrganizationId = c.Int(),
                        OpportunityStatus = c.Int(nullable: false),
                        RejectReason = c.Int(),
                        CreatedBy = c.String(),
                        AssignedTo = c.String(),
                        LastUpdatedBy = c.String(),
                        IsAssigned = c.Boolean(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OpportunityId)
                .ForeignKey("dbo.Campaigns", t => t.RelatedCampaignId)
                .ForeignKey("dbo.Organizations", t => t.RelatedOrganizationId)
                .Index(t => t.RelatedCampaignId)
                .Index(t => t.RelatedOrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Leads", "RelatedOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Leads", "RelatedOpportunityId", "dbo.Opportunities");
            DropForeignKey("dbo.Opportunities", "RelatedOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Opportunities", "RelatedCampaignId", "dbo.Campaigns");
            DropForeignKey("dbo.Leads", "RelatedCampaignId", "dbo.Campaigns");
            DropIndex("dbo.Opportunities", new[] { "RelatedOrganizationId" });
            DropIndex("dbo.Opportunities", new[] { "RelatedCampaignId" });
            DropIndex("dbo.Leads", new[] { "RelatedOrganizationId" });
            DropIndex("dbo.Leads", new[] { "RelatedOpportunityId" });
            DropIndex("dbo.Leads", new[] { "RelatedCampaignId" });
            DropTable("dbo.Opportunities");
            DropTable("dbo.Leads");
        }
    }
}
