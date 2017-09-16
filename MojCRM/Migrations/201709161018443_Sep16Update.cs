namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sep16Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "DoNotCall", c => c.Boolean(nullable: false));
            AddColumn("dbo.MerDeliveryDetails", "AcquiredReceivingInformationIsVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.MerDeliveryDetails", "RequiredPostalService", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MerDeliveryDetails", "RequiredPostalService");
            DropColumn("dbo.MerDeliveryDetails", "AcquiredReceivingInformationIsVerified");
            DropColumn("dbo.Contacts", "DoNotCall");
        }
    }
}
