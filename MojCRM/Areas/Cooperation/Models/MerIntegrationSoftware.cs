using MojCRM.Areas.Stats.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.Cooperation.Models
{
    public class MerIntegrationSoftware
    {
        [Key]
        public int Id { get; set; }
        public int MerId { get; set; }
        public string MerSoftwareID { get; set; }
        public string SoftwareName { get; set; }
        public decimal MerPercentage { get; set; }
        public int? PartnerId { get; set; }
        [ForeignKey("PartnerId")]
        public virtual Organizations Organization { get; set; }

        public virtual ICollection<MerDocumentExchangeHistory> Documents { get; set; }
    }
}