namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul17Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Leads", "LastContactedBy", c => c.String());
            AddColumn("dbo.Leads", "LastContactDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Leads", "LastContactDate");
            DropColumn("dbo.Leads", "LastContactedBy");
        }
    }
}
