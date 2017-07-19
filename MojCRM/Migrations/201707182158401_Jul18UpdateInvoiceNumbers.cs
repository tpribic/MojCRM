namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jul18UpdateInvoiceNumbers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrganizationDetails", "NumberOfInvoicesSent", c => c.String());
            AlterColumn("dbo.OrganizationDetails", "NumberOfInvoicesReceived", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrganizationDetails", "NumberOfInvoicesReceived", c => c.Int());
            AlterColumn("dbo.OrganizationDetails", "NumberOfInvoicesSent", c => c.Int());
        }
    }
}
