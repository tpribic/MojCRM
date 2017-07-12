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

        [Display(Name = "Naziv prodajne prilike")]
        public string OpportunityTitle { get; set; }
        public string OpportunityDescription { get; set; }
        public int? RelatedCampaignId { get; set; }
        public int? RelatedOrganizationId { get; set; }

        [Display(Name = "Status")]
        public OpportunityStatusEnum OpportunityStatus { get; set; }

        [Display(Name = "Razlog odbijanja")]
        public OpportunityRejectReasonEnum? RejectReason { get; set; }

        [Display(Name = "Kreirao")]
        public string CreatedBy { get; set; }

        [Display(Name = "Dodijeljeno agentu")]
        public string AssignedTo { get; set; }

        [Display(Name = "Zadnje kontaktirao")]
        public string LastContactedBy { get; set; }

        [Display(Name = "Zadnje mijenjao")]
        public string LastUpdatedBy { get; set; }

        [Display(Name = "Dodijeljeno")]
        public bool IsAssigned { get; set; }

        [Display(Name = "Datum kreiranja")]
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "Datum zadnjeg kontakta")]
        public DateTime? LastContactDate { get; set; }

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

        public string OpportunityStatusString
        {
            get
            {
                switch(OpportunityStatus)
                {
                    case OpportunityStatusEnum.START: return "Kreirano";
                    case OpportunityStatusEnum.INCONTACT: return "U kontaktu";
                    case OpportunityStatusEnum.LEAD: return "Kreiran lead";
                    case OpportunityStatusEnum.REJECTED: return "Odbijeno";
                }
                return "Status prodajne prilike";
            }
        }

        public string OpportunityRejectReasonString
        {
            get
            {
                switch(RejectReason)
                {
                    case OpportunityRejectReasonEnum.NOINFO: return "Ne želi navesti";
                    case OpportunityRejectReasonEnum.NOINTEREST: return "Nema interesa za uslugu";
                    case OpportunityRejectReasonEnum.PRICE: return "Previsoka cijena";
                    case OpportunityRejectReasonEnum.QUOTE: return "Neadekvatna ponuda";
                    case OpportunityRejectReasonEnum.SERVICEPROVIDER: return "Koristi drugog posrednika";
                }
                return "Razlog odbijanja";
            }
        }
    }
}