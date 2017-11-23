namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jun6Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MerDeliveryDetails", "ImportantComments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MerDeliveryDetails", "ImportantComments");
        }
    }
}
