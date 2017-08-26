namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Aug26Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "LegalForm", c => c.Int(nullable: false));
            AddColumn("dbo.Organizations", "LastUpdatedBy", c => c.String());
            AddColumn("dbo.Organizations", "MerUpdateDate", c => c.DateTime());
            AddColumn("dbo.OrganizationDetails", "MainAddress", c => c.String());
            AddColumn("dbo.OrganizationDetails", "MainPostalCode", c => c.Int(nullable: false));
            AddColumn("dbo.OrganizationDetails", "MainCity", c => c.String());
            AddColumn("dbo.OrganizationDetails", "MainCountry", c => c.Int(nullable: false));
            AddColumn("dbo.OrganizationDetails", "CorrespondenceAddress", c => c.String());
            AddColumn("dbo.OrganizationDetails", "CorrespondencePostalCode", c => c.Int(nullable: false));
            AddColumn("dbo.OrganizationDetails", "CorrespondenceCity", c => c.String());
            AddColumn("dbo.OrganizationDetails", "CorrespondenceCountry", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrganizationDetails", "CorrespondenceCountry");
            DropColumn("dbo.OrganizationDetails", "CorrespondenceCity");
            DropColumn("dbo.OrganizationDetails", "CorrespondencePostalCode");
            DropColumn("dbo.OrganizationDetails", "CorrespondenceAddress");
            DropColumn("dbo.OrganizationDetails", "MainCountry");
            DropColumn("dbo.OrganizationDetails", "MainCity");
            DropColumn("dbo.OrganizationDetails", "MainPostalCode");
            DropColumn("dbo.OrganizationDetails", "MainAddress");
            DropColumn("dbo.Organizations", "MerUpdateDate");
            DropColumn("dbo.Organizations", "LastUpdatedBy");
            DropColumn("dbo.Organizations", "LegalForm");
        }
    }
}
