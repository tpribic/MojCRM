using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    public class Organizations
    {
        [Key]
        [Display(Name = "Moj-eRačun ID")]
        public int MerId { get; set; }

        [Display(Name = "OIB tvrtke")]
        public string VAT { get; set; }

        [Display(Name = "Naziv tvrtke")]
        public string SubjectName { get; set; }

        [Display(Name = "Poslovna jedinica")]
        public string SubjectBusinessUnit { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual MerDeliveryDetails MerDeliveryDetail { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; }
    }
}