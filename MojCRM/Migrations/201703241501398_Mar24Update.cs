namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar24Update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contacts", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Contacts", "Organization_MerId", "dbo.Organizations");
            DropIndex("dbo.Contacts", new[] { "Organization_MerId" });
            DropIndex("dbo.Contacts", new[] { "User_Id" });
            RenameColumn(table: "dbo.Contacts", name: "Organization_MerId", newName: "OrganizationId");
            CreateTable(
                "dbo.DeliveryDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User = c.String(),
                        DetailNote = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        Contact_ContactId = c.Int(),
                        Ticket_Id = c.Int(),
                        Receiver_MerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.Contact_ContactId)
                .ForeignKey("dbo.Deliveries", t => t.Ticket_Id)
                .ForeignKey("dbo.Organizations", t => t.Receiver_MerId)
                .Index(t => t.Contact_ContactId)
                .Index(t => t.Ticket_Id)
                .Index(t => t.Receiver_MerId);
            
            CreateTable(
                "dbo.LogError",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Method = c.String(),
                        Parameters = c.String(),
                        Message = c.String(),
                        InnerException = c.String(),
                        Request = c.String(),
                        User = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Contacts", "User", c => c.String());
            AlterColumn("dbo.Contacts", "OrganizationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Contacts", "OrganizationId");
            AddForeignKey("dbo.Contacts", "OrganizationId", "dbo.Organizations", "MerId", cascadeDelete: true);
            DropColumn("dbo.Contacts", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "User_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.Contacts", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.DeliveryDetails", "Receiver_MerId", "dbo.Organizations");
            DropForeignKey("dbo.DeliveryDetails", "Ticket_Id", "dbo.Deliveries");
            DropForeignKey("dbo.DeliveryDetails", "Contact_ContactId", "dbo.Contacts");
            DropIndex("dbo.DeliveryDetails", new[] { "Receiver_MerId" });
            DropIndex("dbo.DeliveryDetails", new[] { "Ticket_Id" });
            DropIndex("dbo.DeliveryDetails", new[] { "Contact_ContactId" });
            DropIndex("dbo.Contacts", new[] { "OrganizationId" });
            AlterColumn("dbo.Contacts", "OrganizationId", c => c.Int());
            DropColumn("dbo.Contacts", "User");
            DropTable("dbo.LogError");
            DropTable("dbo.DeliveryDetails");
            RenameColumn(table: "dbo.Contacts", name: "OrganizationId", newName: "Organization_MerId");
            CreateIndex("dbo.Contacts", "User_Id");
            CreateIndex("dbo.Contacts", "Organization_MerId");
            AddForeignKey("dbo.Contacts", "Organization_MerId", "dbo.Organizations", "MerId");
            AddForeignKey("dbo.Contacts", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
