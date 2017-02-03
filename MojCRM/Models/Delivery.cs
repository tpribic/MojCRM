using MojCRM.Helpers;
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
    public class Delivery
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Moj-eRačun ID pošiljatelja")]
        public int SenderId { get; set; }

        [Display(Name = "Moj-eRačun ID pošiljatelja")]
        public int? ReceiverId { get; set; }

        [Display(Name = "Interni broj računa")]
        public string InvoiceNumber { get; set; }

        [DefaultValue(1)]
        public int UserId { get; set; }

        public string MerLink { get; set; }
        public MerDeliveryJsonResponse MerJson { get; set; }
        public int MerElectronicId { get; set; }

        [Display(Name = "Datum slanja računa")]
        public DateTime SentDate { get; set; }

        [Display(Name = "Tip dokumenta")]
        public int MerDocumentTypeId { get; set; }

        [Display(Name = "Status dokumenta")]
        public int DocumentStatus { get; set; }

        [Display(Name = "Datum kreiranja kartice")]
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class DeliveryDbContext : ApplicationDbContext
    {
        public DbSet<Delivery> Delivery { get; set; }
    }
}