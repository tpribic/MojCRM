namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul13UpdateLeads : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeadNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelatedLeadId = c.Int(),
                        User = c.String(),
                        Note = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        Contact = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Leads", t => t.RelatedLeadId)
                .Index(t => t.RelatedLeadId);
            
            AddColumn("dbo.OrganizationDetails", "NumberOfInvoicesSent", c => c.Int());
            AddColumn("dbo.OrganizationDetails", "NumberOfInvoicesReceived", c => c.Int());
            AddColumn("dbo.Leads", "LeadDescription", c => c.String());
            AlterColumn("dbo.Leads", "RejectReason", c => c.Int());
            DropColumn("dbo.OrganizationDetails", "NumberOfInvoices");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrganizationDetails", "NumberOfInvoices", c => c.Int());
            DropForeignKey("dbo.LeadNotes", "RelatedLeadId", "dbo.Leads");
            DropIndex("dbo.LeadNotes", new[] { "RelatedLeadId" });
            AlterColumn("dbo.Leads", "RejectReason", c => c.Int(nullable: false));
            DropColumn("dbo.Leads", "LeadDescription");
            DropColumn("dbo.OrganizationDetails", "NumberOfInvoicesReceived");
            DropColumn("dbo.OrganizationDetails", "NumberOfInvoicesSent");
            DropTable("dbo.LeadNotes");
        }
    }
}
