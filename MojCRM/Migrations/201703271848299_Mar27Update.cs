namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar27Update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DeliveryDetails", "Contact_ContactId", "dbo.Contacts");
            DropIndex("dbo.DeliveryDetails", new[] { "Contact_ContactId" });
            AddColumn("dbo.DeliveryDetails", "Contact", c => c.String());
            DropColumn("dbo.DeliveryDetails", "Contact_ContactId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeliveryDetails", "Contact_ContactId", c => c.Int());
            DropColumn("dbo.DeliveryDetails", "Contact");
            CreateIndex("dbo.DeliveryDetails", "Contact_ContactId");
            AddForeignKey("dbo.DeliveryDetails", "Contact_ContactId", "dbo.Contacts", "ContactId");
        }
    }
}
