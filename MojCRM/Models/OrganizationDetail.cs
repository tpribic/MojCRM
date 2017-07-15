﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    public class OrganizationDetail
    {
        [Key, ForeignKey("Organization")]
        public int MerId { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ERP { get; set; }
        public int? NumberOfInvoicesSent { get; set; }
        public int? NumberOfInvoicesReceived { get; set; }

        public virtual Organizations Organization { get; set; }
    }
}