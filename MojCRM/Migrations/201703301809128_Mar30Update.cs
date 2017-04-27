namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar30Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MerUserUsername", c => c.String());
            AddColumn("dbo.AspNetUsers", "MerUserPassword", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MerUserPassword");
            DropColumn("dbo.AspNetUsers", "MerUserUsername");
        }
    }
}
