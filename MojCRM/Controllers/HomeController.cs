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
            var campaign = (from c in db.Campaigns
                            where c.CampaignId == 6
                            select c).First();
            var opportunities = (from o in db.Opportunities
                                 where o.RelatedCampaignId == 6
                                 select o);
            var leads = (from l in db.Leads
                         where l.RelatedCampaignId == 6
                         select l);

            var countModel = new GeneralCampaignStatusViewModelCount()
            {
                NumberOfOpportunitiesCreated = opportunities.Count(),
                NumberOfOpportunitiesInProgress = opportunities.Where(o => o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.START || o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.ARRANGEMEETING || o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.INCONTACT || o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.PROCESSDIFFICULTIES).Count(),
                NumberOfOpportunitiesToLead = opportunities.Where(o => o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.LEAD).Count(),
                NumberOfOpportunitiesRejected = opportunities.Where(o => o.OpportunityStatus == Areas.Sales.Models.Opportunity.OpportunityStatusEnum.REJECTED).Count(),
                NumberOfLeadsCreated = leads.Count(),
                NumberOfLeadsInProgress = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.INCONTACT || l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.MEETING || l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.QOUTESENT || l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.START).Count(),
                NumberOfLeadsMeetings = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.MEETING).Count(),
                NumberOfLeadsQuotes = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.QOUTESENT).Count(),
                NumberOfLeadsRejected = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.REJECTED).Count(),
                NumberOfLeadsAccepted = leads.Where(l => l.LeadStatus == Areas.Sales.Models.Lead.LeadStatusEnum.ACCEPTED).Count()
            };

            var model = new GeneralCampaignStatusViewModel()
            {
                RelatedCampaignId = 6,
                RelatedCampaignName = campaign.CampaignName,
                NumberOfOpportunitiesCreated = countModel.NumberOfOpportunitiesCreated,
                NumberOfOpportunitiesInProgress = countModel.NumberOfOpportunitiesInProgress,
                NumberOfOpportunitiesInProgressPercent = Math.Round((((decimal)countModel.NumberOfOpportunitiesInProgress / (decimal)countModel.NumberOfOpportunitiesCreated) * 100), 2),
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
            return View(model);
        }
    }
}
