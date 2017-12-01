namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aug26Update2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganizationAttributes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        AttributeClass = c.Int(nullable: false),
                        AttributeType = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        AssignedBy = c.String(),
                        InsertDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.OrganizationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganizationAttributes", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.OrganizationAttributes", new[] { "OrganizationId" });
            DropTable("dbo.OrganizationAttributes");
        }
    }
}
