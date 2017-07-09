namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul3UpdateCampaigns : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Campaigns", "UpdateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Campaigns", "UpdateDate", c => c.DateTime(nullable: false));
        }
    }
}
