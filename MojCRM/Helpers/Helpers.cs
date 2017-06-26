using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojCRM.Helpers
{
    public class ChangeEmailNoTicket
    {
        public int MerElectronicId { get; set; }
        public int ReceiverId { get; set; }

        [Display(Name = "E-mail adresa primatelja:")]
        public string OldEmail { get; set; }
        public int TicketId { get; set; }
        public string Agent { get; set; }
    }
}