namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar12UpdatedContactsv2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserContacts", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserContacts", "Contact_ContactId", "dbo.Contacts");
            DropIndex("dbo.ApplicationUserContacts", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserContacts", new[] { "Contact_ContactId" });
            AddColumn("dbo.Contacts", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Contacts", "User_Id");
            AddForeignKey("dbo.Contacts", "User_Id", "dbo.AspNetUsers", "Id");
            DropTable("dbo.ApplicationUserContacts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserContacts",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Contact_ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Contact_ContactId });
            
            DropForeignKey("dbo.Contacts", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Contacts", new[] { "User_Id" });
            DropColumn("dbo.Contacts", "User_Id");
            CreateIndex("dbo.ApplicationUserContacts", "Contact_ContactId");
            CreateIndex("dbo.ApplicationUserContacts", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserContacts", "Contact_ContactId", "dbo.Contacts", "ContactId", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserContacts", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
