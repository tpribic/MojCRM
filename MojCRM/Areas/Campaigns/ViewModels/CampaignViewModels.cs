﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Campaigns.Helpers;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;

namespace MojCRM.Areas.Campaigns.ViewModels
{
    public class CampaignDetailsViewModel
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public Campaign Campaign { get; set; }
        public EmailBasesCampaignStatsViewModel EmailBasesStats { get; set; }
        public SalesCampaignStatsViewModel SalesStats { get; set; }
        public int NumberOfUnassignedEntities { get; set; }
        public IQueryable<CampaignMember> AssignedMembers { get; set; }
        public IQueryable<CampaignAssignedAgents> AssignedAgents { get; set; }

        public IQueryable<SelectListItem> CampaignStatusList
        {
            get
            {
                var statusList = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi status kampanje --"},
                    new SelectListItem {Value = "1", Text = @"U tijeku"},
                    new SelectListItem {Value = "2", Text = @"Privremeno zaustavljeno"},
                    new SelectListItem {Value = "3", Text = @"Prekinuto"},
                    new SelectListItem {Value = "4", Text = @"Završeno"}
                };
                return statusList.AsQueryable();
            }
        }

        public IQueryable<SelectListItem> MemberRoleList
        {
            get
            {
                var roleList = new List<SelectListItem>
                {
                    new SelectListItem {Value = null, Text = @"-- Odaberi rolu agenta u kampanji --"},
                    new SelectListItem {Value = "0", Text = @"Nositelj kampanje"},
                    new SelectListItem {Value = "1", Text = @"Nadzornik kampanje"},
                    new SelectListItem {Value = "2", Text = @"Član kampanje"}
                };
                return roleList.AsQueryable();
            }
        }
        public IQueryable<SelectListItem> AgentList
        {
            get
            {
                var agents = (from a in _db.Users
                    select a);

                var agentsList = new List<SelectListItem>();

                foreach (var agent in agents)
                {
                    agentsList.Add(new SelectListItem { Value = agent.UserName, Text = agent.UserName });
                }

                return agentsList.AsQueryable();
            }
        }

        public int GetUnassignedEntities(int campaignId)
        {
            var number = _db.AcquireEmails.Count(x => x.Campaign.CampaignId == campaignId && x.AcquireEmailStatus == AcquireEmail.AcquireEmailStatusEnum.Created && x.IsAssigned == false);
            return number;
        }

        public IQueryable<CampaignAssignedAgents> GetAssignedAgentsInfo(int campaignId)
        {
            var agents = _db.Users.Select(x => x.UserName);
            var model = new List<CampaignAssignedAgents>();

            foreach (var agent in agents)
            {
                var temp = new CampaignAssignedAgents()
                {
                    Agent = agent,
                    NumberOfAssignedEntities =
                        _db.AcquireEmails.Count(e => e.AssignedTo == agent && e.RelatedCampaignId == campaignId)
                };
                model.Add(temp);
            }

            return model.AsQueryable();
        }
    }
}