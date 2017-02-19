namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb18UpdateOrganizationBU : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "SubjectBusinessUnit", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "SubjectBusinessUnit");
        }
    }
}
