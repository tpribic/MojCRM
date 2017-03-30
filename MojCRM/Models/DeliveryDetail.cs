using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    [Table("DeliveryDetails")]
    public class DeliveryDetail
    {
        [Key]
        public int Id { get; set; }

        public int? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual Organizations Receiver { get; set; }
        public string User { get; set; }
        public string DetailNote { get; set; }
        public DateTime InsertDate { get; set; }
        public string Contact { get; set; }
        public int? TicketId { get; set; }
        [ForeignKey("TicketId")]
        public virtual Delivery Ticket { get; set; }
    }
}