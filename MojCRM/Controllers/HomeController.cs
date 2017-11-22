using System;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Areas.Campaigns.ViewModels;
using MojCRM.Areas.Sales.Models;
using MojCRM.Areas.Stats.ViewModels;
using MojCRM.Models;
using MojCRM.ViewModels;

namespace MojCRM.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var campaignIna = (from c in _db.Campaigns
                            where c.CampaignId == 1
                            select c).First();
            var opportunities = (from o in _db.Opportunities
                                 where o.RelatedCampaignId == 1
                                 select o);
            var leads = (from l in _db.Leads
                         where l.RelatedCampaignId == 1
                         select l);

            var countModel = new GeneralCampaignStatusViewModelCount
            {
                NumberOfOpportunitiesCreated = opportunities.Count(),
                NumberOfOpportunitiesInProgress = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.START || o.OpportunityStatus == Opportunity.OpportunityStatusEnum.ARRANGEMEETING || o.OpportunityStatus == Opportunity.OpportunityStatusEnum.INCONTACT || o.OpportunityStatus == Opportunity.OpportunityStatusEnum.PROCESSDIFFICULTIES),
                NumberOfOpportunitiesUser = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.MERUSER),
                NumberOfOpportunitiesToLead = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.LEAD),
                NumberOfOpportunitiesRejected = opportunities.Count(o => o.OpportunityStatus == Opportunity.OpportunityStatusEnum.REJECTED),
                NumberOfLeadsCreated = leads.Count(),
                NumberOfLeadsInProgress = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.INCONTACT || l.LeadStatus == Lead.LeadStatusEnum.MEETING || l.LeadStatus == Lead.LeadStatusEnum.START),
                NumberOfLeadsMeetings = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.MEETING),
                NumberOfLeadsQuotes = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.QOUTESENT),
                NumberOfLeadsRejected = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.REJECTED),
                NumberOfLeadsAccepted = leads.Count(l => l.LeadStatus == Lead.LeadStatusEnum.ACCEPTED)
            };

            var modelIna = new GeneralCampaignStatusViewModel
            {
                RelatedCampaignId = 6,
                RelatedCampaignName = campaignIna.CampaignName,
                NumberOfOpportunitiesCreated = countModel.NumberOfOpportunitiesCreated,
                NumberOfOpportunitiesInProgress = countModel.NumberOfOpportunitiesInProgress,
                NumberOfOpportunitiesInProgressPercent = Math.Round(((countModel.NumberOfOpportunitiesInProgress / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitesUser = countModel.NumberOfOpportunitiesUser,
                NumberOfOpportunitiesUserPercent = Math.Round(((countModel.NumberOfOpportunitiesUser / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitiesToLead = countModel.NumberOfOpportunitiesToLead,
                NumberOfOpportunitiesToLeadPercent = Math.Round(((countModel.NumberOfOpportunitiesToLead / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfOpportunitiesRejected = countModel.NumberOfOpportunitiesRejected,
                NumberOfOpportunitiesRejectedPercent = Math.Round(((countModel.NumberOfOpportunitiesRejected / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
                NumberOfLeadsCreated = countModel.NumberOfLeadsCreated,
                NumberOfLeadsInProgress = countModel.NumberOfLeadsInProgress,
                NumberOfLeadsInProgressPercent = Math.Round(((countModel.NumberOfLeadsInProgress / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsMeetings = countModel.NumberOfLeadsMeetings,
                NumberOfLeadsMeetingsPercent = Math.Round(((countModel.NumberOfLeadsMeetings / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsQuotes = countModel.NumberOfLeadsQuotes,
                NumberOfLeadsQuotesPercent = Math.Round(((countModel.NumberOfLeadsQuotes / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsRejected = countModel.NumberOfLeadsRejected,
                NumberOfLeadsRejectedPercent = Math.Round(((countModel.NumberOfLeadsRejected / (decimal)countModel.NumberOfLeadsCreated) * 100), 2),
                NumberOfLeadsAccepted = countModel.NumberOfLeadsAccepted,
                NumberOfLeadsAcceptedPercent = Math.Round(((countModel.NumberOfLeadsAccepted / (decimal)countModel.NumberOfLeadsCreated) * 100), 2)
            };

            var campaignsModel = new EmailBasesCampaignStatsViewModel();
            var campaignMemberModel = new CampaignMember();
            var agentActivities = new CallCenterDailyStatsViewModel();

            var model = new HomeViewModel
            {
                INACampaign = modelIna,
                Campaigns = campaignsModel.GetModels(),
                CampaignMembers = campaignMemberModel.GetCamapigns(User.Identity.Name),
                AgentActivities = agentActivities.GetActivitiesForDashboard()
            };
            return View(model);
        }
    }
}
