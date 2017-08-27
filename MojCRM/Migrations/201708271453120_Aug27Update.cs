namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aug27Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "ServiceProvider", c => c.Int(nullable: false));
            AddColumn("dbo.MerDeliveryDetails", "TotalSent", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MerDeliveryDetails", "TotalSent");
            DropColumn("dbo.Organizations", "ServiceProvider");
        }
    }
}
