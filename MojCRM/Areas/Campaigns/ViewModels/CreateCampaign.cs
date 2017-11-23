using System;
using System.ComponentModel.DataAnnotations;
using static MojCRM.Areas.Campaigns.Models.Campaign;

namespace MojCRM.Areas.Campaigns.ViewModels
{
    public class CreateCampaign
    {
        [Display(Name = "Naziv kampanje")]
        public string CampaignName { get; set; }

        [Display(Name = "Opis kampanje")]
        public string CampaignDescription { get; set; }

        [Display(Name = "Pokrenuo")]
        public string CampaignInitiator { get; set; }

        [Display(Name = "ID tvrtke")]
        public int RelatedCompanyId { get; set; }

        [Display(Name = "Tip")]
        public CampaignTypeEnum CampaignType { get; set; }

        [Display(Name = "Početak")]
        public DateTime CampaignStartDate { get; set; }

        [Display(Name = "Predviđeni završetak")]
        public DateTime CampaignPlannedEndDate { get; set; }
    }
}