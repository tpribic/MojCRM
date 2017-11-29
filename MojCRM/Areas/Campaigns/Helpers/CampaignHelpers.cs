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

    public class EmailBasesCampaignStatusHelper
    {
        public string StatusName { get; set; }
        public int SumOfEntities { get; set; }
    }

    public class SalesOpportunitiesCampaignStatusHelper
    {
        public string StatusName { get; set; }
        public int SumOfEntities { get; set; }
    }

    public class SalesLeadsCampaignStatusHelper
    {
        public string StatusName { get; set; }
        public int SumOfEntities { get; set; }
    }
}