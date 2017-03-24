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

        public virtual Organizations Receiver { get; set; }
        public string User { get; set; }
        public DateTime InsertDate { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual Delivery Ticket { get; set; }
    }
}