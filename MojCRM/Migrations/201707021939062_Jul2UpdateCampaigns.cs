namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul2UpdateCampaigns : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CampaignMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CampaignId = c.Int(nullable: false),
                        MemberName = c.String(),
                        MemberRole = c.Int(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Campaigns", t => t.CampaignId, cascadeDelete: true)
                .Index(t => t.CampaignId);
            
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        CampaignId = c.Int(nullable: false, identity: true),
                        CampaignName = c.String(),
                        CampaignDescription = c.String(),
                        CampaignInitiatior = c.String(),
                        RelatedCompanyId = c.Int(nullable: false),
                        CampaignType = c.Int(nullable: false),
                        CampaignStatus = c.Int(nullable: false),
                        CampaignStartDate = c.DateTime(nullable: false),
                        CampaignPlannedEndDate = c.DateTime(nullable: false),
                        CampaignEndDate = c.DateTime(nullable: false),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CampaignId)
                .ForeignKey("dbo.Organizations", t => t.RelatedCompanyId, cascadeDelete: true)
                .Index(t => t.RelatedCompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CampaignMembers", "CampaignId", "dbo.Campaigns");
            DropForeignKey("dbo.Campaigns", "RelatedCompanyId", "dbo.Organizations");
            DropIndex("dbo.Campaigns", new[] { "RelatedCompanyId" });
            DropIndex("dbo.CampaignMembers", new[] { "CampaignId" });
            DropTable("dbo.Campaigns");
            DropTable("dbo.CampaignMembers");
        }
    }
}
