namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class May26UpdateDeliveryAssign : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deliveries", "IsAssigned", c => c.Boolean(nullable: false));
            AddColumn("dbo.Deliveries", "AssignedTo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Deliveries", "AssignedTo");
            DropColumn("dbo.Deliveries", "IsAssigned");
        }
    }
}
