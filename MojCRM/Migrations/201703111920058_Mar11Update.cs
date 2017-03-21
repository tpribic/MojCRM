namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar11Update : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.DeliveryDetailsViewModels");
        }
        
        public override void Down()
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
    }
}
