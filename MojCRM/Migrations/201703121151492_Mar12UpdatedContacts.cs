namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar12UpdatedContacts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrganizationsContacts", "Organizations_MerId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationsContacts", "Contact_ContactId", "dbo.Contacts");
            DropForeignKey("dbo.ApplicationUserContacts", "Contact_ContactId", "dbo.Contacts");
            DropIndex("dbo.OrganizationsContacts", new[] { "Organizations_MerId" });
            DropIndex("dbo.OrganizationsContacts", new[] { "Contact_ContactId" });
            DropPrimaryKey("dbo.Contacts");
            AddColumn("dbo.Contacts", "ContactType", c => c.String());
            AlterColumn("dbo.Contacts", "ContactId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Contacts", "ContactId");
            CreateIndex("dbo.Contacts", "ContactId");
            AddForeignKey("dbo.Contacts", "ContactId", "dbo.Organizations", "MerId");
            AddForeignKey("dbo.ApplicationUserContacts", "Contact_ContactId", "dbo.Contacts", "ContactId", cascadeDelete: true);
            DropTable("dbo.OrganizationsContacts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OrganizationsContacts",
                c => new
                    {
                        Organizations_MerId = c.Int(nullable: false),
                        Contact_ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Organizations_MerId, t.Contact_ContactId });
            
            DropForeignKey("dbo.ApplicationUserContacts", "Contact_ContactId", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "ContactId", "dbo.Organizations");
            DropIndex("dbo.Contacts", new[] { "ContactId" });
            DropPrimaryKey("dbo.Contacts");
            AlterColumn("dbo.Contacts", "ContactId", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.Contacts", "ContactType");
            AddPrimaryKey("dbo.Contacts", "ContactId");
            CreateIndex("dbo.OrganizationsContacts", "Contact_ContactId");
            CreateIndex("dbo.OrganizationsContacts", "Organizations_MerId");
            AddForeignKey("dbo.ApplicationUserContacts", "Contact_ContactId", "dbo.Contacts", "ContactId", cascadeDelete: true);
            AddForeignKey("dbo.OrganizationsContacts", "Contact_ContactId", "dbo.Contacts", "ContactId", cascadeDelete: true);
            AddForeignKey("dbo.OrganizationsContacts", "Organizations_MerId", "dbo.Organizations", "MerId", cascadeDelete: true);
        }
    }
}
