namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb7Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserFirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "UserLastName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "UserLastName");
            DropColumn("dbo.AspNetUsers", "UserFirstName");
        }
    }
}
