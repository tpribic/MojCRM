namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul11Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganizationDetails",
                c => new
                    {
                        MerId = c.Int(nullable: false),
                        TelephoneNumber = c.String(),
                        MobilePhoneNumber = c.String(),
                        EmailAddress = c.String(),
                        ERP = c.String(),
                        NumberOfInvoices = c.Int(),
                    })
                .PrimaryKey(t => t.MerId)
                .ForeignKey("dbo.Organizations", t => t.MerId)
                .Index(t => t.MerId);
            
            CreateTable(
                "dbo.OpportunityNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedOpportunityId = c.Int(),
                        User = c.String(),
                        Note = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        Contact = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Opportunities", t => t.RelatedOpportunityId)
                .Index(t => t.RelatedOpportunityId);
            
            AddColumn("dbo.Opportunities", "OpportunityDescription", c => c.String());
            AddColumn("dbo.Opportunities", "LastContactedBy", c => c.String());
            AddColumn("dbo.Opportunities", "LastContactDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OpportunityNotes", "RelatedOpportunityId", "dbo.Opportunities");
            DropForeignKey("dbo.OrganizationDetails", "MerId", "dbo.Organizations");
            DropIndex("dbo.OpportunityNotes", new[] { "RelatedOpportunityId" });
            DropIndex("dbo.OrganizationDetails", new[] { "MerId" });
            DropColumn("dbo.Opportunities", "LastContactDate");
            DropColumn("dbo.Opportunities", "LastContactedBy");
            DropColumn("dbo.Opportunities", "OpportunityDescription");
            DropTable("dbo.OpportunityNotes");
            DropTable("dbo.OrganizationDetails");
        }
    }
}
