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

        [Display(Name = "Prvi primljeni dokument")]
        public DateTime? FirstReceived { get; set; }

        [Display(Name = "Prvi poslani dokument")]
        public DateTime? FirstSent { get; set; }

        [Display(Name = "Broj zaposlenih")]
        public int? Employees { get; set; }

        [Display(Name = "Ukupan godišnji prihod")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [DataType(DataType.Currency)]
        public decimal? Income { get; set; }

        [Display(Name = "Datum unosa")]
        public DateTime? InsertDate { get; set; }

        [Display(Name = "Datum izmijene")]
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual MerDeliveryDetails MerDeliveryDetail { get; set; }
        public virtual OrganizationDetail OrganizationDetail { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; }
    }
}