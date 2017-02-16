namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb16Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deliveries", "BuyerEmail", c => c.String());
            AddColumn("dbo.MerDeliveryJsonResponses", "EmailPrimatelja", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MerDeliveryJsonResponses", "EmailPrimatelja");
            DropColumn("dbo.Deliveries", "BuyerEmail");
        }
    }
}
