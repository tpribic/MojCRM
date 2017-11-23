using MojCRM.Areas.Cooperation.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.Stats.Models
{
    public class MerDocumentExchangeHistory
    {
        [Key]
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Month { get; set; }
        public int DocumentType { get; set; }
        public bool IsOutgoing { get; set; }
        public int Count { get; set; }
        public int SoftwareId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organizations Organization { get; set; }
        [ForeignKey("SoftwareId")]
        public virtual MerIntegrationSoftware Software { get; set; }
    }
}