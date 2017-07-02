using MojCRM.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MojCRM.Areas.Campaigns.Models
{
    public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }

        public string CampaignName { get; set; }
        public string CampaignDescription { get; set; }
        public string CampaignInitiatior { get; set; }
        public int RelatedCompanyId { get; set; }

        [ForeignKey("RelatedCompanyId")]
        public virtual Organizations RelatedCompany { get; set; }

        public CampaignTypeEnum CampaignType { get; set; }
        public CampaignStatusEnum CampaignStatus { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public DateTime CampaignPlannedEndDate { get; set; }
        public DateTime CampaignEndDate { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public enum CampaignTypeEnum
        {
            [Description("Test")]
            TEST,

            [Description("Prikupljanje e-mail adresa")]
            EMAILBASES,

            [Description("Prodajna kampanja")]
            SALES,

            [Description("CRM kampanja")]
            CRM
        }

        public enum CampaignStatusEnum
        {
            [Description("Pokrenuto")]
            START,

            [Description("U tijeku")]
            INPROGRESS,

            [Description("Privremeno zaustavljeno")]
            HOLD,

            [Description("Prekinuto")]
            ENDED,

            [Description("Završeno")]
            COMPLETED
        }
    }
}