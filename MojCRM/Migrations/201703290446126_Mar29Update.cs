namespace MojCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mar29Update : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DeliveryDetails", name: "Receiver_MerId", newName: "ReceiverId");
            RenameColumn(table: "dbo.DeliveryDetails", name: "Ticket_Id", newName: "TicketId");
            RenameIndex(table: "dbo.DeliveryDetails", name: "IX_Receiver_MerId", newName: "IX_ReceiverId");
            RenameIndex(table: "dbo.DeliveryDetails", name: "IX_Ticket_Id", newName: "IX_TicketId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.DeliveryDetails", name: "IX_TicketId", newName: "IX_Ticket_Id");
            RenameIndex(table: "dbo.DeliveryDetails", name: "IX_ReceiverId", newName: "IX_Receiver_MerId");
            RenameColumn(table: "dbo.DeliveryDetails", name: "TicketId", newName: "Ticket_Id");
            RenameColumn(table: "dbo.DeliveryDetails", name: "ReceiverId", newName: "Receiver_MerId");
        }
    }
}
