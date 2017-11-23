﻿using MojCRM.Helpers;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.HelpDesk.Models
{
    public class Delivery
    {
        [Key]
        public int Id { get; set; }

        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        [Display(Name = "Pošiljatelj")]
        public virtual Organizations Sender { get; set; }

        public int? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        [Display(Name = "Primatelj")]
        public virtual Organizations Receiver { get; set; }

        [Display(Name = "Interni broj računa")]
        public string InvoiceNumber { get; set; }
        public int UserId { get; set; }

        public string MerLink { get; set; }
        public MerDeliveryJsonResponse MerJson { get; set; }
        public int MerElectronicId { get; set; }

        [Display(Name = "Datum slanja dokumenta")]
        [DataType(DataType.Date)]
        public DateTime SentDate { get; set; }

        [Display(Name = "Tip dokumenta")]
        public int MerDocumentTypeId { get; set; }
        public int DocumentStatus { get; set; }
        public string BuyerEmail { get; set; }

        [Display(Name = "Datum kreiranja kartice")]
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Prvi račun")]
        public bool FirstInvoice { get; set; }

        [Display(Name = "Dodijeljeno")]
        public bool IsAssigned { get; set; }

        [Display(Name = "Dodijeljeno agentu")]
        public string AssignedTo { get; set; }

        [Display(Name = "Datum i vrijeme zadnje provjere povijesti dokumenata")]
        public DateTime? LastGetSentDocumentsDate { get; set; }

        [Display(Name = "Povijest primljenih dokumenata")]
        public string GetSentDocumentsResult { get; set; }

        public virtual ICollection<DeliveryDetail> DeliveryDetails { get; set; }

        [NotMapped]
        [Display(Name = "Status dokumenta")]
        public string DocumentStatusString
        {
            get
            {
                switch (DocumentStatus)
                {
                    case 10: return "U pripremi";
                    case 20: return "Potpisan";
                    case 30: return "Poslan";
                    case 40: return "Dostavljen";
                    case 45: return "Ispisan";
                    case 50: return "Neuspješan";
                    case 55: return "Uklonjen";
                }
                return "Status";
            }
        }

        [NotMapped]
        [Display(Name = "Tip dokumenta")]
        public string MerDocumentTypeIdString
        {
            get
            {
                switch (MerDocumentTypeId)
                {
                    case 0: return "eDokument";
                    case 1: return "eRačun";
                    case 3: return "Storno";
                    case 4: return "eOpomena";
                    case 6: return "ePrimka - tip 6";
                    case 7: return "eOdgovor";
                    case 105: return "eNarudžba";
                    case 226: return "eOpoziv";
                    case 230: return "eIzmjena";
                    case 231: return "eOdgovorN";
                    case 351: return "eOtpremnica";
                    case 381: return "eOdobrenje";
                    case 383: return "eTerećenje";
                }
                return "Tip dokumenta";
            }
        }
    }
}