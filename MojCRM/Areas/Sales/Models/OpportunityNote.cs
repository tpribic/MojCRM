using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class OpportunityNote
    {
        [Key]
        public int Id { get; set; }

        public int? RelatedOpportunityId { get; set; }
        public string User { get; set; }
        public string Note { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Contact { get; set; }

        [ForeignKey("RelatedOpportunityId")]
        public virtual Opportunity Opportunity { get; set; }
    }
}