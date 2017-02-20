﻿using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.ViewModels
{
    public class DeliveryDetailsViewModel
    {
        [Key]
        public int? TicketId { get; set; }

        [Display(Name = "Pošiljatelj:")]
        public string SenderName { get; set; }

        [Display(Name = "Primatelj:")]
        public string ReceiverName { get; set; }

        [NotMapped]
        public int? ReceiverId { get; set; }

        [Display(Name = "Interni broj dokumenta:")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Datum i vrijeme slanja dokumenta:")]
        [DataType(DataType.DateTime)]
        public DateTime SentDate { get; set; }
        public int MerDocumentTypeId { get; set; }

        [Display(Name = "E-mail adresa primatelja:")]
        public string ReceiverEmail { get; set; }

        [Display(Name = "Napomena:")]
        public string MerDeliveryDetailComment { get; set; }

        [Display(Name = "Kontakt podaci:")]
        public string MerDeliveryDetailTelephone { get; set; }


        [Display(Name = "Tip dokumenta:")]
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
                    case 7: return "eOdgovor";
                    case 105: return "eNarudžba";
                    case 226: return "eOpoziv";
                    case 230: return "eIzmjena";
                    case 231: return "eOdgovorN";
                    case 351: return "eOtrpemnica";
                    case 381: return "eOdobrenje";
                    case 383: return "eTerećenje";
                }
                return "Tip dokumenta";
            }
        }

        [NotMapped]
        public List<Delivery> UndeliveredInvoices { get; set; }
    }
}