namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Feb18UpdateViewModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeliveryDetailsViewModels",
                c => new
                    {
                        TicketId = c.Int(nullable: false, identity: true),
                        SenderName = c.String(),
                        ReceiverName = c.String(),
                        InvoiceNumber = c.String(),
                        SentDate = c.DateTime(nullable: false),
                        MerDocumentTypeId = c.Int(nullable: false),
                        ReceiverEmail = c.String(),
                        MerDeliveryDetailComment = c.String(),
                        MerDeliveryDetailTelephone = c.String(),
                    })
                .PrimaryKey(t => t.TicketId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DeliveryDetailsViewModels");
        }
    }
}
