namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class May12UpdateContacts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contacts", "ContactType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contacts", "ContactType", c => c.String());
        }
    }
}
