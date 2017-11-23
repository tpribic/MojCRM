namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul13UpdateLeadsv2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Leads", "RelatedOpportunityId", "dbo.Opportunities");
            DropIndex("dbo.Leads", new[] { "RelatedOpportunityId" });
            AlterColumn("dbo.Leads", "RelatedOpportunityId", c => c.Int(nullable: false));
            CreateIndex("dbo.Leads", "RelatedOpportunityId");
            AddForeignKey("dbo.Leads", "RelatedOpportunityId", "dbo.Opportunities", "OpportunityId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Leads", "RelatedOpportunityId", "dbo.Opportunities");
            DropIndex("dbo.Leads", new[] { "RelatedOpportunityId" });
            AlterColumn("dbo.Leads", "RelatedOpportunityId", c => c.Int());
            CreateIndex("dbo.Leads", "RelatedOpportunityId");
            AddForeignKey("dbo.Leads", "RelatedOpportunityId", "dbo.Opportunities", "OpportunityId");
        }
    }
}
