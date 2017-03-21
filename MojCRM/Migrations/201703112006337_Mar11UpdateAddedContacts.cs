namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar11UpdateAddedContacts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ContactId = c.Int(nullable: false, identity: true),
                        ContactFirstName = c.String(),
                        ContactLastName = c.String(),
                        Title = c.String(),
                        TelephoneNumber = c.String(),
                        MobilePhoneNumber = c.String(),
                        Email = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ContactId);
            
            CreateTable(
                "dbo.OrganizationsContacts",
                c => new
                    {
                        Organizations_MerId = c.Int(nullable: false),
                        Contact_ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Organizations_MerId, t.Contact_ContactId })
                .ForeignKey("dbo.Organizations", t => t.Organizations_MerId, cascadeDelete: true)
                .ForeignKey("dbo.Contacts", t => t.Contact_ContactId, cascadeDelete: true)
                .Index(t => t.Organizations_MerId)
                .Index(t => t.Contact_ContactId);
            
            CreateTable(
                "dbo.ApplicationUserContacts",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Contact_ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Contact_ContactId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Contacts", t => t.Contact_ContactId, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Contact_ContactId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserContacts", "Contact_ContactId", "dbo.Contacts");
            DropForeignKey("dbo.ApplicationUserContacts", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationsContacts", "Contact_ContactId", "dbo.Contacts");
            DropForeignKey("dbo.OrganizationsContacts", "Organizations_MerId", "dbo.Organizations");
            DropIndex("dbo.ApplicationUserContacts", new[] { "Contact_ContactId" });
            DropIndex("dbo.ApplicationUserContacts", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.OrganizationsContacts", new[] { "Contact_ContactId" });
            DropIndex("dbo.OrganizationsContacts", new[] { "Organizations_MerId" });
            DropTable("dbo.ApplicationUserContacts");
            DropTable("dbo.OrganizationsContacts");
            DropTable("dbo.Contacts");
        }
    }
}
