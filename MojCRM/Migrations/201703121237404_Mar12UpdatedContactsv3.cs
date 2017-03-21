namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar12UpdatedContactsv3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Contacts", "ContactId", "dbo.Organizations");
            DropIndex("dbo.Contacts", new[] { "ContactId" });
            DropPrimaryKey("dbo.Contacts");
            AddColumn("dbo.Contacts", "Organization_MerId", c => c.Int());
            AlterColumn("dbo.Contacts", "ContactId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Contacts", "UpdateDate", c => c.DateTime());
            AddPrimaryKey("dbo.Contacts", "ContactId");
            CreateIndex("dbo.Contacts", "Organization_MerId");
            AddForeignKey("dbo.Contacts", "Organization_MerId", "dbo.Organizations", "MerId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contacts", "Organization_MerId", "dbo.Organizations");
            DropIndex("dbo.Contacts", new[] { "Organization_MerId" });
            DropPrimaryKey("dbo.Contacts");
            AlterColumn("dbo.Contacts", "UpdateDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Contacts", "ContactId", c => c.Int(nullable: false));
            DropColumn("dbo.Contacts", "Organization_MerId");
            AddPrimaryKey("dbo.Contacts", "ContactId");
            CreateIndex("dbo.Contacts", "ContactId");
            AddForeignKey("dbo.Contacts", "ContactId", "dbo.Organizations", "MerId");
        }
    }
}
