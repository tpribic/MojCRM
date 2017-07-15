﻿using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Sales.Models
{
    public class Lead
    {
        [Key]
        public int LeadId { get; set; }
        public string LeadTitle { get; set; }
        public string LeadDescription { get; set; }
        public int? RelatedCampaignId { get; set; }
        [ForeignKey("RelatedOpportunity")]
        public int RelatedOpportunityId { get; set; }

        public int? RelatedOrganizationId { get; set; }
        public LeadStatusEnum LeadStatus { get; set; }
        public LeadRejectReasonEnum? RejectReason { get; set; }
        public QuoteTypeEnum? QuoteType { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool IsAssigned { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [ForeignKey("RelatedCampaignId")]
        public virtual Campaign RelatedCampaign { get; set; }
        //[ForeignKey("RelatedOpportunityId")]
        public virtual Opportunity RelatedOpportunity { get; set; }
        [ForeignKey("RelatedOrganizationId")]
        public virtual Organizations RelatedOrganization { get; set; }

        public enum LeadStatusEnum
        {
            [Description("Početno")]
            START,

            [Description("U kontaktu")]
            INCONTACT,

            [Description("Odbijeno")]
            REJECTED,

            [Description("Poslana ponuda")]
            QOUTESENT,

            [Description("Prihvaćena ponuda")]
            ACCEPTED
        }

        public enum LeadRejectReasonEnum
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

        public enum QuoteTypeEnum
        {
            [Description("PrePaid paket (Moj-eRačun)")]
            ADVANSEeR,

            [Description("Ugovor (Moj-eRačun)")]
            CONTRACTeR,

            [Description("Paket (Moj-DMS)")]
            PACKAGEDMS,

            [Description("Paket (Moj-eArhiv)")]
            PACKAGEeA,

            [Description("Paket (Moj-eRačun + Moj-DMS)")]
            PACKAGEeRDMS,

            [Description("Paket (Moj-eRačun + Moj-eArhiv)")]
            PACKAGEeReA,

            [Description("Paket (Moj-eArhiv + Moj-DMS")]
            PACKAGEeADMS,

            [Description("Bundle paket (Moj-eRačun + Moj-DMS + Moj-eArhiv")]
            BUNDLE

        }

        public string LeadStatusString
        {
            get
            {
                switch(LeadStatus)
                {
                    case LeadStatusEnum.START: return "Kreirano";
                    case LeadStatusEnum.INCONTACT: return "U kontaktu";
                    case LeadStatusEnum.REJECTED: return "Odbijeno";
                    case LeadStatusEnum.QOUTESENT: return "Poslana ponuda";
                    case LeadStatusEnum.ACCEPTED: return "Ponuda prihvaćena";
                }
                return "Status leada";
            }
        }

        public string LeadRejectReasonString
        {
            get
            {
                switch (RejectReason)
                {
                    case LeadRejectReasonEnum.NOINFO: return "Ne želi navesti";
                    case LeadRejectReasonEnum.NOINTEREST: return "Nema interesa za uslugu";
                    case LeadRejectReasonEnum.PRICE: return "Previsoka cijena";
                    case LeadRejectReasonEnum.QUOTE: return "Neadekvatna ponuda";
                    case LeadRejectReasonEnum.SERVICEPROVIDER: return "Koristi drugog posrednika";
                }
                return "Nije odbijeno";
            }
        }
    }
}