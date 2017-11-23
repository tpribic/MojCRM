namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sep2Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MerDocumentExchangeHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        Month = c.String(),
                        DocumentType = c.Int(nullable: false),
                        IsOutgoing = c.Boolean(nullable: false),
                        Count = c.Int(nullable: false),
                        SoftwareId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.MerIntegrationSoftwares", t => t.SoftwareId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.SoftwareId);
            
            CreateTable(
                "dbo.MerIntegrationSoftwares",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MerId = c.Int(nullable: false),
                        MerSoftwareID = c.String(),
                        SoftwareName = c.String(),
                        MerPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PartnerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.PartnerId)
                .Index(t => t.PartnerId);
            
            AddColumn("dbo.Organizations", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "IsPartner", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "HasSLA", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationDetails", "MerSendSoftware", c => c.Int());
            AddColumn("dbo.OrganizationDetails", "MerReceiveSoftware", c => c.Int());
            CreateIndex("dbo.OrganizationDetails", "MerSendSoftware");
            CreateIndex("dbo.OrganizationDetails", "MerReceiveSoftware");
            AddForeignKey("dbo.OrganizationDetails", "MerReceiveSoftware", "dbo.MerIntegrationSoftwares", "Id");
            AddForeignKey("dbo.OrganizationDetails", "MerSendSoftware", "dbo.MerIntegrationSoftwares", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganizationDetails", "MerSendSoftware", "dbo.MerIntegrationSoftwares");
            DropForeignKey("dbo.OrganizationDetails", "MerReceiveSoftware", "dbo.MerIntegrationSoftwares");
            DropForeignKey("dbo.MerIntegrationSoftwares", "PartnerId", "dbo.Organizations");
            DropForeignKey("dbo.MerDocumentExchangeHistories", "SoftwareId", "dbo.MerIntegrationSoftwares");
            DropForeignKey("dbo.MerDocumentExchangeHistories", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.OrganizationDetails", new[] { "MerReceiveSoftware" });
            DropIndex("dbo.OrganizationDetails", new[] { "MerSendSoftware" });
            DropIndex("dbo.MerIntegrationSoftwares", new[] { "PartnerId" });
            DropIndex("dbo.MerDocumentExchangeHistories", new[] { "SoftwareId" });
            DropIndex("dbo.MerDocumentExchangeHistories", new[] { "OrganizationId" });
            DropColumn("dbo.OrganizationDetails", "MerReceiveSoftware");
            DropColumn("dbo.OrganizationDetails", "MerSendSoftware");
            DropColumn("dbo.Organizations", "HasSLA");
            DropColumn("dbo.Organizations", "IsPartner");
            DropColumn("dbo.Organizations", "IsActive");
            DropTable("dbo.MerIntegrationSoftwares");
            DropTable("dbo.MerDocumentExchangeHistories");
        }
    }
}
