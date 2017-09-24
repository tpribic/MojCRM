using MojCRM.Areas.Campaigns.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Areas.Sales.Models;
using MojCRM.Models;

namespace MojCRM.Areas.Campaigns.ViewModels
{
    public class SalesCampaignStatsViewModel
    {
        public Campaign Campaign { get; set; }
        public int TotalCount { get; set; }
        public int StartedCount { get; set; }
        public int NotStartedCount { get; set; }
        public decimal NotStartedPercent { get; set; }
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public SalesCampaignStatsViewModel GetModel(int id)
        {
            var campaign = _db.Campaigns.First(c => c.CampaignType == Campaign.CampaignTypeEnum.SALES && c.CampaignId == id);

            var newCampaign = new SalesCampaignStatsViewModel()
            {
                Campaign = campaign,
                TotalCount = _db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId),
                StartedCount = _db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.START),
                NotStartedCount = _db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId && op.OpportunityStatus != Opportunity.OpportunityStatusEnum.START),
                NotStartedPercent = Math.Round(((_db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId && op.OpportunityStatus != Opportunity.OpportunityStatusEnum.START)
                                               / (decimal)_db.Opportunities.Count(op => op.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
            };

            return newCampaign;
        }
    }

    public class EmailBasesCampaignStatsViewModel
    {
        public Campaign Campaign { get; set; }
        public int TotalCount { get; set; }
        public int NotVerifiedCount { get; set; }
        public decimal CreatedPercent { get; set; }
        public decimal CheckedPercent { get; set; }
        public decimal VerifiedPercent { get; set; }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public List<EmailBasesCampaignStatsViewModel> GetModels()
        {
            var campaignsTemp = _db.Campaigns.Where(c => c.CampaignType == Campaign.CampaignTypeEnum.EMAILBASES);
            var list = new List<EmailBasesCampaignStatsViewModel>();

            foreach (var campaign in campaignsTemp)
            {
                var newCampaign = new EmailBasesCampaignStatsViewModel()
                {
                    Campaign = campaign,
                    TotalCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId),
                    NotVerifiedCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus != AcquireEmail.AcquireEmailStatusEnum.VERIFIED),
                    CreatedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.CREATED)
                                                  / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                    CheckedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.CHECKED)
                                                  / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                    VerifiedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.VERIFIED)
                                                   / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                };
                list.Add(newCampaign);
            }

            return list;
        }
        public EmailBasesCampaignStatsViewModel GetModel(int id)
        {
            var campaign = _db.Campaigns.First(c => c.CampaignType == Campaign.CampaignTypeEnum.EMAILBASES && c.CampaignId == id);

            var newCampaign = new EmailBasesCampaignStatsViewModel()
            {
                Campaign = campaign,
                TotalCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId),
                NotVerifiedCount = _db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus != AcquireEmail.AcquireEmailStatusEnum.VERIFIED),
                CreatedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.CREATED)
                                              / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                CheckedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.CHECKED)
                                              / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
                VerifiedPercent = Math.Round(((_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.VERIFIED)
                                               / (decimal)_db.AcquireEmails.Count(a => a.RelatedCampaignId == campaign.CampaignId)) * 100), 0),
            };

            return newCampaign;
        }
    }
}