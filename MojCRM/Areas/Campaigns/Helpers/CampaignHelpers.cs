using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.Campaigns.Helpers
{
    public class CampaignSearchHelper
    {
        public string Organization { get; set; }
        public string CampaignName { get; set; }
    }

    public class CampaignAssignedAgents
    {
        public string Agent { get; set; }
        public int NumberOfAssignedEntities { get; set; }
    }
}