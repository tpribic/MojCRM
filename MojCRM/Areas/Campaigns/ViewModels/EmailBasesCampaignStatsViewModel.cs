using MojCRM.Areas.Campaigns.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojCRM.Areas.Campaigns.ViewModels
{
    public class EmailBasesCampaignStatsViewModel
    {
        public Campaign Campaign { get; set; }
        public int TotalCount { get; set; }
        public int NotVerifiedCount { get; set; }
        public decimal CreatedPercent { get; set; }
        public decimal CheckedPercent { get; set; }
        public decimal VerifiedPercent { get; set; }
    }
}