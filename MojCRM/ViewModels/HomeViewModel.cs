using MojCRM.Areas.Campaigns.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.ViewModels
{
    public class HomeViewModel
    { 
        public GeneralCampaignStatusViewModel INACampaign { get; set; }
        public IList<EmailBasesCampaignStatsViewModel> Campaigns { get; set; }
    }
}