﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.Helpers
{
    public class DeliverySearchHelper
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string InvoiceNumber { get; set; }
        public string SentDate { get; set; }
        public string TicketDate { get; set; }
        public string BuyerEmail { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentType { get; set; }
        public string TicketType { get; set; }
        public string Assigned { get; set; }
        public string AssignedTo { get; set; }
    }

    public class ChangeEmailHelper
    {
        public int MerElectronicId { get; set; }
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string InvoiceNumber { get; set; }
    }

    public class DeliveryDetailHelper
    {
        public int ReceiverId { get; set; }
        public int TicketId { get; set; }
        public string Contact { get; set; }
        public string DetailTemplate { get; set; }
        public string DetailNote { get; set; }
        public string InvoiceNumber { get; set; }
        public int? DetailNoteId { get; set; }
        public int? Identifier { get; set; }
    }
}