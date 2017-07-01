namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul1OrganizationsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "FirstSent", c => c.DateTime());
            AddColumn("dbo.Organizations", "InsertDate", c => c.DateTime());
            AddColumn("dbo.Organizations", "UpdateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "UpdateDate");
            DropColumn("dbo.Organizations", "InsertDate");
            DropColumn("dbo.Organizations", "FirstSent");
        }
    }
}
