using MojCRM.Areas.Campaigns.ViewModels;
using System.Collections.Generic;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Areas.Stats.ViewModels;

namespace MojCRM.ViewModels
{
    public class HomeViewModel
    { 
        public GeneralCampaignStatusViewModel INACampaign { get; set; }
        public IList<EmailBasesCampaignStatsViewModel> Campaigns { get; set; }
        public IList<CampaignMember> CampaignMembers { get; set; }
        public IList<CallCenterDaily> AgentActivities { get; set; }
    }
}