namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Apr10Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliveryDetails", "UpdateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliveryDetails", "UpdateDate");
        }
    }
}
