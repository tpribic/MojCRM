﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Models;

namespace MojCRM.Areas.Stats.ViewModels
{
    public class DailyDelivery
    {
        public DateTime ReferenceDate { get; set; }
        public int CreatedTicketsCount { get; set; }
        public int CreatedTicketsFirstTimeCount { get; set; }
        public int AssignedToCount { get; set; }
        public int SentCount { get; set; }
        public int DeliveredCount { get; set; }
        public int OtherCount { get; set; }
        public string AssignedTo { get; set; }
    }
    public class DeliveryStatsViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int CreatedTicketsTodayCount { get; set; }
        public int CreatedTicketsTodayFirstTimeCount { get; set; }
        public IList<DailyDelivery> Deliveries { get; set; }
        public IList<SelectListItem> Agents
        {
            get
            {
                var agents = (from u in db.Users
                              select new SelectListItem()
                              {
                                  Text = u.UserName,
                                  Value = u.UserName
                              }).ToList();
                return agents;
            }
        }
    }
    public class AssigningTickets
    {
        public DateTime? TicketDate { get; set; }
        public DateTime SentDate { get; set; }
        public string Agent { get; set; }
    }

    public class CallCenterDaily
    {
        [Display(Name = "Agent")]
        public string Agent { get; set; }
        [Display(Name = "Broj uspješnih poziva")]
        public int NumberSuccessfulCalls { get; set; }
        [Display(Name = "Broj neuspješnih poziva")]
        public int NumberUnsuccessfulCalls { get; set; }
        [Display(Name = "Broj ispravaka mailova")]
        public int? NumberMailchange { get; set; }
        [Display(Name = "Broj ponovno poslanih obavijesti o dostavi")]
        public int? NumberResend { get; set; }
        [Display(Name = "Broj poslanih e-mailova vezanih za dostavu")]
        public int? NumberMail { get; set; }
        [Display(Name = "Broj zaključanih kartica (Odjel dostave)")]
        public int? NumberTicketsAssigned { get; set; }
    }
    public class CallCenterDailyByDepartment
    {
        [Display(Name = "Odjel")]
        public ActivityLog.DepartmentEnum Department { get; set; }
        [Display(Name = "Zbroj uspješnih poziva")]
        public int NumberSuccessfulCalls { get; set; }
        [Display(Name = "Zbroj neuspješnih poziva")]
        public int NumberUnsuccessfulCalls { get; set; }
        [Display(Name = "Zbroj poslanih e-mailova korisnicima")]
        public int? NumberMail { get; set; }
        [Display(Name = "Zbroj ispravaka mailova (DOSTAVA)")]
        public int? NumberMailchange { get; set; }
        [Display(Name = "Zbroj ponovno poslanih obavijesti o dostavi (DOSTAVA)")]
        public int? NumberResend { get; set; }
        

        public string DepartmentString
        {
            get
            {
                switch (Department)
                {
                    case ActivityLog.DepartmentEnum.MojCRM: return "Moj-CRM";
                    case ActivityLog.DepartmentEnum.Delivery: return "Odjel dostave eRačuna";
                    case ActivityLog.DepartmentEnum.Sales: return "Odjel prodaje";
                }
                return "Odjel";
            }
        }
    }
    public class CallCenterDailyStatsViewModel
    {
        public IList<CallCenterDaily> Activities { get; set; }
        public IList<CallCenterDailyByDepartment> ActivitiesByDepartment { get; set; }
        [Display(Name = "Uspješni pozivi")]
        public int SumSuccessfulCalls { get; set; }
        [Display(Name = "Neuspješni pozivi")]
        public int SumUnsuccessfulCalls { get; set; }
        [Display(Name = "Poslani e-mailovi vezani za dostavu")]
        public int? SumSentMail { get; set; }
        [Display(Name = "Ispravci e-mailova (Odjel dostave)")]
        public int? SumMailchange { get; set; }
        [Display(Name = "Ponovno poslane obavijesti o dostavi (Odjel dostave)")]
        public int? SumResend { get; set; }
        [Display(Name = "Zaključane kartice (Odjel dostave)")]
        public int? SumTicketsAssigned { get; set; }

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public IList<CallCenterDaily> GetActivitiesForDashboard()
        {
            var referenceDate = DateTime.Now.Date;
            var activities = new List<CallCenterDaily>();
            var successfulCalls = _db.ActivityLogs.Where(a => (a.InsertDate >= referenceDate) && (a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALL || a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALSHORT));
            var unsuccessfulCalls = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.UNSUCCAL));
            var mailChange = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.MAILCHANGE));
            var resend = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.RESEND));
            var deliveryMail = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.EMAIL));
            var ticketsAssigned = _db.ActivityLogs.Where(t => (t.InsertDate >= referenceDate) && (t.ActivityType == ActivityLog.ActivityTypeEnum.TICKETASSIGN));

            var agentActivities = (from a in _db.ActivityLogs
                               where (a.InsertDate >= referenceDate)
                               group a by a.User into ga
                               select ga).ToList();
            foreach (var day in agentActivities)
            {
                var dailyActivities = new CallCenterDaily
                {
                    Agent = day.Key,
                    NumberSuccessfulCalls = successfulCalls.Count(a => a.User == day.Key),
                    NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.User == day.Key),
                    NumberMailchange = mailChange.Count(a => a.User == day.Key),
                    NumberResend = resend.Count(a => a.User == day.Key),
                    NumberMail = deliveryMail.Count(a => a.User == day.Key),
                    NumberTicketsAssigned = ticketsAssigned.Count(a => a.User == day.Key)
                };
                activities.Add(dailyActivities);
            }

            return activities;
        }
    }

    public class PersonalDailyActivitiesViewModel
    {
        public IList<ActivityLog> PersonalActivities { get; set; }
        public IEnumerable<ApplicationUser> Agents { get; set; }
        [Display(Name = "Ukupno uspješnih poziva")]
        public int SumSuccessfulCalls { get; set; }
        [Display(Name = "Ukupno uspješnih kratkih poziva")]
        public int SumShortSuccessfulCalls { get; set; }
        [Display(Name = "Ukupno neuspješnih poziva")]
        public int SumUnsuccessfulCalls { get; set; }
        [Display(Name = "Ukupno poslanih e-mailova korisnicima")]
        public int? SumSentMail { get; set; }
        [Display(Name = "Ukupno ispravaka mailova")]
        public int? SumMailchange { get; set; }
        [Display(Name = "Ukupno ponovno poslanih obavijesti o dostavi")]
        public int? SumResend { get; set; }
        public IList<SelectListItem> AgentList
        {
            get
            {
                var listAgents = (from u in Agents
                                  select new SelectListItem()
                                  {
                                      Text = u.UserName,
                                      Value = u.UserName
                                  }).ToList();
                return listAgents;
            }
        }
    }

    public class GeneralCampaignStatusViewModel
    {
        public int RelatedCampaignId { get; set; }
        public string RelatedCampaignName { get; set; }
        public int NumberOfOpportunitiesCreated { get; set; }
        public int NumberOfOpportunitiesInProgress { get; set; }
        public decimal NumberOfOpportunitiesInProgressPercent { get; set; }
        public int NumberOfOpportunitesUser { get; set; }
        public decimal NumberOfOpportunitiesUserPercent { get; set; }
        public int NumberOfOpportunitiesToLead { get; set; }
        public decimal NumberOfOpportunitiesToLeadPercent { get; set; }
        public int NumberOfOpportunitiesRejected { get; set; }
        public decimal NumberOfOpportunitiesRejectedPercent { get; set; }
        public int NumberOfLeadsCreated { get; set; }
        public int NumberOfLeadsInProgress { get; set; }
        public decimal NumberOfLeadsInProgressPercent { get; set; }
        public int NumberOfLeadsMeetings { get; set; }
        public decimal NumberOfLeadsMeetingsPercent { get; set; }
        public int NumberOfLeadsQuotes { get; set; }
        public decimal NumberOfLeadsQuotesPercent { get; set; }
        public int NumberOfLeadsRejected { get; set; }
        public decimal NumberOfLeadsRejectedPercent { get; set; }
        public int NumberOfLeadsAccepted { get; set; }
        public decimal NumberOfLeadsAcceptedPercent { get; set; }
    }
    public class GeneralCampaignStatusViewModelCount
    {
        public int NumberOfOpportunitiesCreated { get; set; }
        public int NumberOfOpportunitiesInProgress { get; set; }
        public int NumberOfOpportunitiesUser { get; set; }
        public int NumberOfOpportunitiesToLead { get; set; }
        public int NumberOfOpportunitiesRejected { get; set; }
        public int NumberOfLeadsCreated { get; set; }
        public int NumberOfLeadsInProgress { get; set; }
        public int NumberOfLeadsMeetings { get; set; }
        public int NumberOfLeadsQuotes { get; set; }
        public int NumberOfLeadsRejected { get; set; }
        public int NumberOfLeadsAccepted { get; set; }
    }
}