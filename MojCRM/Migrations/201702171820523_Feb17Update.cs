namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb17Update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MerDeliveryDetails",
                c => new
                    {
                        MerId = c.Int(nullable: false),
                        Comments = c.String(),
                        Telephone = c.String(),
                    })
                .PrimaryKey(t => t.MerId)
                .ForeignKey("dbo.Organizations", t => t.MerId)
                .Index(t => t.MerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MerDeliveryDetails", "MerId", "dbo.Organizations");
            DropIndex("dbo.MerDeliveryDetails", new[] { "MerId" });
            DropTable("dbo.MerDeliveryDetails");
        }
    }
}
