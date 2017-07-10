using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class Opportunity
    {
        [Key]
        public int OpportunityId { get; set; }
        public string OpportunityTitle { get; set; }
        public int? RelatedCampaignId { get; set; }
        public int? RelatedOrganizationId { get; set; }
        public OpportunityStatusEnum OpportunityStatus { get; set; }
        public OpportunityRejectReasonEnum? RejectReason { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool IsAssigned { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign RelatedCampaign { get; set; }
        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations RelatedOrganization { get; set; }

        public enum OpportunityStatusEnum
        {
            [Description("Početno")]
            START,

            [Description("U kontaktu")]
            INCONTACT,

            [Description("Lead")]
            LEAD,

            [Description("Odbijeno")]
            REJECTED
        }

        public enum OpportunityRejectReasonEnum
        {
            [Description("Ne želi navesti")]
            NOINFO,

            [Description("Nema interesa")]
            NOINTEREST,

            [Description("Previsoka cijena")]
            PRICE,

            [Description("Neadekvatna ponuda")]
            QUOTE,

            [Description("Drugi pružatelj usluga")]
            SERVICEPROVIDER
        }
    }
}