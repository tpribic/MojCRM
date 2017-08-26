using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Display(Name = "Pravni oblik")]
        public LegalFormEnum LegalForm { get; set; }

        [Display(Name = "Zadnju promjenu obavio")]
        public string LastUpdatedBy { get; set; }

        [Display(Name = "Datum unosa")]
        public DateTime? InsertDate { get; set; }

        [Display(Name = "Datum izmijene")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Datum zadnje sinkronizacije s Moj-eRačun")]
        public DateTime? MerUpdateDate { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual MerDeliveryDetails MerDeliveryDetail { get; set; }
        public virtual OrganizationDetail OrganizationDetail { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; }

        public enum LegalFormEnum
        {
            [Description("Nije navedeno")]
            NOINFO,

            [Description("Društvo s ograničenom odgovornošću")]
            DOO,

            [Description("Jednostavno društvo s ograničenom odgovornošću")]
            JDOO,

            [Description("Dioničko društvo")]
            DD,

            [Description("Ostala trgovačka društva (Komanditno društvo, Javno trgovačko društvo")]
            KDJTD,

            [Description("Obrt")]
            OBRT,

            [Description("Ostali pravni oblici (Zadruge, OPG, Udruge, Ustanove i sl.")]
            OTHER
        }
    }
}