using MojCRM.Areas.Campaigns.ViewModels;
using MojCRM.Models;
using MojCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MojCRM.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var campaignINA = (from c in db.Campaigns
                            where c.CampaignId == 1
                            select c).First();
            var opportunities = (from o in db.Opportunities
                                 where o.RelatedCampaignId == 1
                                 select o);
            var leads = (from l in db.Leads
                         where l.RelatedCampaignId == 1
                         select l);

            var countModel = new GeneralCampaignStatusViewModelCount()
            {
                NumberOfOpportunitiesCreated = opportunities.Count(),
                NumberOfOpportunitiesInProgress = opportunities.Where(o => o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.START || o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.ARRANGEMEETING || o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.INCONTACT || o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.PROCESSDIFFICULTIES).Count(),
                NumberOfOpportunitiesUser = opportunities.Where(o => o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.MERUSER).Count(),
                NumberOfOpportunitiesToLead = opportunities.Where(o => o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.LEAD).Count(),
                NumberOfOpportunitiesRejected = opportunities.Where(o => o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.REJECTED).Count(),
                NumberOfLeadsCreated = leads.Count(),
                NumberOfLeadsInProgress = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.INCONTACT || l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.MEETING || l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.START).Count(),
                NumberOfLeadsMeetings = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.MEETING).Count(),
                NumberOfLeadsQuotes = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.QOUTESENT).Count(),
                NumberOfLeadsRejected = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.REJECTED).Count(),
                NumberOfLeadsAccepted = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.ACCEPTED).Count()
            };

            var modelINA = new GeneralCampaignStatusViewModel()
            {
                RelatedCampaignId = 6,
                RelatedCampaignName = campaignINA.CampaignName,
                NumberOfOpportunitiesCreated = countModel.NumberOfOpportunitiesCreated,
                NumberOfOpportunitiesInProgress = countModel.NumberOfOpportunitiesInProgress,
                NumberOfOpportunitiesInProgressPercent = Math.Round((((decimal)countModel.NumberOfOpportunitiesInProgress / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitesUser = countModel.NumberOfOpportunitiesUser,
                NumberOfOpportunitiesUserPercent = Math.Round((((decimal)countModel.NumberOfOpportunitiesUser / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitiesToLead = countModel.NumberOfOpportunitiesToLead,
                NumberOfOpportunitiesToLeadPercent = Math.Round((((decimal)countModel.NumberOfOpportunitiesToLead / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitiesRejected = countModel.NumberOfOpportunitiesRejected,
                NumberOfOpportunitiesRejectedPercent = Math.Round((((decimal)countModel.NumberOfOpportunitiesRejected / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfLeadsCreated = countModel.NumberOfLeadsCreated,
                NumberOfLeadsInProgress = countModel.NumberOfLeadsInProgress,
                NumberOfLeadsInProgressPercent = Math.Round((((decimal)countModel.NumberOfLeadsInProgress / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsMeetings = countModel.NumberOfLeadsMeetings,
                NumberOfLeadsMeetingsPercent = Math.Round((((decimal)countModel.NumberOfLeadsMeetings / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsQuotes = countModel.NumberOfLeadsQuotes,
                NumberOfLeadsQuotesPercent = Math.Round((((decimal)countModel.NumberOfLeadsQuotes / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsRejected = countModel.NumberOfLeadsRejected,
                NumberOfLeadsRejectedPercent = Math.Round((((decimal)countModel.NumberOfLeadsRejected / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsAccepted = countModel.NumberOfLeadsAccepted,
                NumberOfLeadsAcceptedPercent = Math.Round((((decimal)countModel.NumberOfLeadsAccepted / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
            };

            var campaignsTemp = db.Campaigns.Where(c => c.CampaignType == Areas.Campaigns.Models.Campaign.CampaignTypeEnum.EMAILBASES);
            var campaigns = new List<EmailBasesCampaignStatsViewModel>();
            foreach (var campaign in campaignsTemp)
            {
                var newCampaign = new EmailBasesCampaignStatsViewModel()
                {
                    Campaign = campaign,
                    TotalCount = db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId).Count(),
                    NotVerifiedCount = db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus != Areas.HelpDesk.Models.AcquireEmail.AcquireEmailStatusEnum.VERIFIED).Count(),
                    CreatedPercent = Math.Round((((decimal)db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == Areas.HelpDesk.Models.AcquireEmail.AcquireEmailStatusEnum.CREATED).Count() 
                    / (decimal)db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId).Count()) * 100), 0),
                    CheckedPercent = Math.Round((((decimal)db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == Areas.HelpDesk.Models.AcquireEmail.AcquireEmailStatusEnum.CHECKED).Count()
                    / (decimal)db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId).Count()) * 100), 0),
                    VerifiedPercent =  Math.Round((((decimal)db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId && a.AcquireEmailStatus == Areas.HelpDesk.Models.AcquireEmail.AcquireEmailStatusEnum.VERIFIED).Count()
                    / (decimal)db.AcquireEmails.Where(a => a.RelatedCampaignId == campaign.CampaignId).Count()) * 100), 0),
                };
                campaigns.Add(newCampaign);
            }

            var model = new HomeViewModel()
            {
                INACampaign = modelINA,
                Campaigns = campaigns
            };
            return View(model);
        }
    }
}
