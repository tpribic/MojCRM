namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sep24Update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CampaignMembers", "UpdateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CampaignMembers", "UpdateDate", c => c.DateTime(nullable: false));
        }
    }
}
