using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MojCRM.Areas.Stats.ViewModels;

namespace MojCRM.Areas.Stats.Controllers
{
    public class StatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stats
        public ActionResult Index()
        {
            return View();
        }

        // GET: Stats/PersonalDailyActivities
        public ActionResult PersonalDailyActivities(string Name, string Agent, string SearchDate)
        {


            var SuccessfulCalls = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALL 
                                   select a);
            var ShortSuccessfulCalls = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALSHORT 
                                   select a);
            var UnsuccessfulCalls = (from a in db.ActivityLogs
                                     where a.ActivityType == ActivityLog.ActivityTypeEnum.UNSUCCAL
                                     select a);
            var MailChange = (from a in db.ActivityLogs
                              where a.ActivityType == ActivityLog.ActivityTypeEnum.MAILCHANGE
                              select a);
            var Resend = (from a in db.ActivityLogs
                          where a.ActivityType == ActivityLog.ActivityTypeEnum.RESEND
                          select a);
            var DeliveryMail = (from a in db.ActivityLogs
                                where a.ActivityType == ActivityLog.ActivityTypeEnum.EMAIL
                                select a);

            var ReferenceDate = DateTime.Today.AddDays(-1);
           
            var _Agents = (from u in db.Users
                           select u);
           
            var _Activities = (from a in db.ActivityLogs
                               select a);

          
            var searchDate = Convert.ToDateTime(SearchDate);
           
            var searchDatePlus = searchDate.AddDays(1);

            var _DistinctDepartments = (from a in db.ActivityLogs
                                        where (a.User == Name) && (a.InsertDate >= DateTime.Today)
                                        select a.Department).Distinct().Count();
            
            if (String.IsNullOrEmpty(SearchDate))
            {
                _Activities = _Activities.Where(a => (a.User == Name) && /*(a.InsertDate >= ReferenceDate) && (a.InsertDate < DateTime.Today)*/ (a.InsertDate>= DateTime.Today));

                SuccessfulCalls = SuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today && a.User == Name);
                ShortSuccessfulCalls = ShortSuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today && a.User == Name);

                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);
                MailChange = MailChange.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);
                Resend = Resend.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);
                DeliveryMail = DeliveryMail.Where(t => t.InsertDate >= DateTime.Today && t.User == Name);



                ViewBag.DistinctDepartments = _DistinctDepartments;
                ViewBag.Date = DateTime.Today.ToShortDateString();
            }
            if (!String.IsNullOrEmpty(SearchDate) && String.IsNullOrEmpty(Agent))
            {

                SuccessfulCalls = SuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Name);
                ShortSuccessfulCalls = ShortSuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Name);

                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                MailChange = MailChange.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                Resend = Resend.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                DeliveryMail = DeliveryMail.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Name);
                _Activities = _Activities.Where(a => (a.User == Name) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));
                
                _DistinctDepartments = (from a in db.ActivityLogs
                                        where (a.User == Name) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                        select a.Department).Distinct().Count();
                ViewBag.Date = searchDate.ToShortDateString();
                ViewBag.DistinctDepartments = _DistinctDepartments;
            }
            if (!String.IsNullOrEmpty(SearchDate) && !String.IsNullOrEmpty(Agent))
            {
                
                SuccessfulCalls = SuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Agent);
                ShortSuccessfulCalls = ShortSuccessfulCalls.Where(a => ((a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)) && a.User == Agent);

                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Agent);
                MailChange = MailChange.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Agent);
                Resend = Resend.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Agent);
                DeliveryMail = DeliveryMail.Where(t => ((t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus)) && t.User == Agent);

                _Activities = _Activities.Where(a => (a.User == Agent) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));

                _DistinctDepartments = (from a in db.ActivityLogs
                                        where (a.User == Agent) && (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                        select a.Department).Distinct().Count();
                ViewBag.DistinctDepartments = _DistinctDepartments;
                ViewBag.Date = searchDate.ToShortDateString();
            }

            if (!String.IsNullOrEmpty(Agent))
            {
                ViewBag.User = Agent;
            }
            else
            {
                ViewBag.User = Name;
            }

            var PersonalActivities = new PersonalDailyActivitiesViewModel
            {
                PersonalActivities = _Activities,
                Agents = _Agents,
                SumSuccessfulCalls = SuccessfulCalls.Count(),
                SumShortSuccessfulCalls = ShortSuccessfulCalls.Count(),
                SumUnsuccessfulCalls = UnsuccessfulCalls.Count(),
                SumMailchange = MailChange.Count(),
                SumResend = Resend.Count(),
                SumSentMail = DeliveryMail.Count(),
               
            };

            return View(PersonalActivities);
        }

        // GET: Stats/CallCenterDaily
        public ActionResult CallCenterDaily(string SearchDate)
        {
            var AgentActivities = (from a in db.ActivityLogs
                                   where a.InsertDate >= DateTime.Today
                                   group a by a.User into ga
                                   select ga).ToList();
            var DepartmentActivities = (from a in db.ActivityLogs
                                        where a.InsertDate >= DateTime.Today
                                        group a by a.Department into ga
                                        select ga).ToList();
            var SuccessfulCalls = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALL || a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALSHORT
                                   select a);
            var UnsuccessfulCalls = (from a in db.ActivityLogs
                                     where a.ActivityType == ActivityLog.ActivityTypeEnum.UNSUCCAL
                                     select a);
            var MailChange = (from a in db.ActivityLogs
                              where a.ActivityType == ActivityLog.ActivityTypeEnum.MAILCHANGE
                              select a);
            var Resend = (from a in db.ActivityLogs
                          where a.ActivityType == ActivityLog.ActivityTypeEnum.RESEND
                          select a);
            var DeliveryMail = (from a in db.ActivityLogs
                                where a.ActivityType == ActivityLog.ActivityTypeEnum.EMAIL
                                select a);
            var TicketsAssigned = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.TICKETASSIGN
                                   select a);
            var Activities = new List<CallCenterDaily>();
            var ActivitiesByDepartment = new List<CallCenterDailyByDepartment>();

            if (!String.IsNullOrEmpty(SearchDate))
            {
                var searchDate = Convert.ToDateTime(SearchDate);
                var searchDatePlus = searchDate.AddDays(1);
                SuccessfulCalls = SuccessfulCalls.Where(a => (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));
                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                MailChange = MailChange.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                Resend = Resend.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                DeliveryMail = DeliveryMail.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                TicketsAssigned = TicketsAssigned.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));

                AgentActivities = (from a in db.ActivityLogs
                                   where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                   group a by a.User into ga
                                   select ga).ToList();
                DepartmentActivities = (from a in db.ActivityLogs
                                        where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                        group a by a.Department into ga
                                        select ga).ToList();
                foreach (var Day in AgentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = Day.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.User == Day.Key).Count(),
                        NumberResend = Resend.Where(a => a.User == Day.Key).Count(),
                        NumberMail = DeliveryMail.Where(a => a.User == Day.Key).Count(),
                        NumberTicketsAssigned = TicketsAssigned.Where(a => a.User == Day.Key).Count()
                    };
                    Activities.Add(dailyActivities);
                }
                foreach (var Department in DepartmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterDailyByDepartment
                    {
                        Department = Department.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.Department == Department.Key).Count(),
                        NumberResend = Resend.Where(a => a.Department == Department.Key).Count(),
                        NumberMail = DeliveryMail.Where(a => a.Department == Department.Key).Count()
                    };
                    ActivitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }
            else
            {
                SuccessfulCalls = SuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today);
                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today);
                MailChange = MailChange.Where(t => t.InsertDate >= DateTime.Today);
                Resend = Resend.Where(t => t.InsertDate >= DateTime.Today);
                DeliveryMail = DeliveryMail.Where(t => t.InsertDate >= DateTime.Today);
                TicketsAssigned = TicketsAssigned.Where(t => t.InsertDate >= DateTime.Today);
                foreach (var Day in AgentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = Day.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.User == Day.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.User == Day.Key).Count(),
                        NumberResend = Resend.Where(a => a.User == Day.Key).Count(),
                        NumberMail = DeliveryMail.Where(a => a.User == Day.Key).Count(),
                        NumberTicketsAssigned = TicketsAssigned.Where(a => a.User == Day.Key).Count()
                    };
                    Activities.Add(dailyActivities);
                }
                foreach (var Department in DepartmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterDailyByDepartment
                    {
                        Department = Department.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Where(a => a.Department == Department.Key).Count(),
                        NumberMailchange = MailChange.Where(a => a.Department == Department.Key).Count(),
                        NumberResend = Resend.Where(a => a.Department == Department.Key).Count(),
                        NumberMail = DeliveryMail.Where(a => a.Department == Department.Key).Count()
                    };
                    ActivitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }

            var model = new CallCenterDailyStatsViewModel
            {
                Activities = Activities.AsQueryable(),
                ActivitiesByDepartment = ActivitiesByDepartment.AsQueryable(),
                SumSuccessfulCalls = SuccessfulCalls.Count(),
                SumUnsuccessfulCalls = UnsuccessfulCalls.Count(),
                SumMailchange = MailChange.Count(),
                SumResend = Resend.Count(),
                SumSentMail = DeliveryMail.Count(),
                SumTicketsAssigned = TicketsAssigned.Count()
            };

            return View(model);
        }

        // GET: Stats/CallCenterWeekly
        public ActionResult CallCenterWeekly(string search)
        {

            var agentActivities = (from a in db.ActivityLogs
                                   where a.InsertDate >= DateTime.Today
                                   group a by a.User into ga
                                   select ga).ToList();
            var departmentActivities = (from a in db.ActivityLogs
                                        where a.InsertDate >= DateTime.Today
                                        group a by a.Department into ga
                                        select ga).ToList();
            var successfulCalls = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALL || a.ActivityType == ActivityLog.ActivityTypeEnum.SUCCALSHORT
                                   select a);
            var unsuccessfulCalls = (from a in db.ActivityLogs
                                     where a.ActivityType == ActivityLog.ActivityTypeEnum.UNSUCCAL
                                     select a);
            var mailChange = (from a in db.ActivityLogs
                              where a.ActivityType == ActivityLog.ActivityTypeEnum.MAILCHANGE
                              select a);
            var resend = (from a in db.ActivityLogs
                          where a.ActivityType == ActivityLog.ActivityTypeEnum.RESEND
                          select a);
            var deliveryMail = (from a in db.ActivityLogs
                                where a.ActivityType == ActivityLog.ActivityTypeEnum.EMAIL
                                select a);
            var ticketsAssigned = (from a in db.ActivityLogs
                                   where a.ActivityType == ActivityLog.ActivityTypeEnum.TICKETASSIGN
                                   select a);
            var activities = new List<CallCenterWeekly>();
            var activitiesByDepartment = new List<CallCenterWeeklyByDepartment>();

            if (!String.IsNullOrEmpty(search))
            {
                var searchDate = Convert.ToDateTime(search);
                var searchDatePlus = searchDate.AddDays(1);
                successfulCalls = successfulCalls.Where(a => (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));
                unsuccessfulCalls = unsuccessfulCalls.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                mailChange = mailChange.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                resend = resend.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                deliveryMail = deliveryMail.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                ticketsAssigned = ticketsAssigned.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));

                agentActivities = (from a in db.ActivityLogs
                                   where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                   group a by a.User into ga
                                   select ga).ToList();
                departmentActivities = (from a in db.ActivityLogs
                                        where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                        group a by a.Department into ga
                                        select ga).ToList();
                foreach (var day in agentActivities)
                {
                    var dailyActivities = new CallCenterWeekly
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
                foreach (var department in departmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterWeeklyByDepartment
                    {
                        Department = department.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.Department == department.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.Department == department.Key),
                        NumberMailchange = mailChange.Count(a => a.Department == department.Key),
                        NumberResend = resend.Count(a => a.Department == department.Key),
                        NumberMail = deliveryMail.Count(a => a.Department == department.Key)
                    };
                    activitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }
            else
            {
                successfulCalls = successfulCalls.Where(a => a.InsertDate >= DateTime.Today);
                unsuccessfulCalls = unsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today);
                mailChange = mailChange.Where(t => t.InsertDate >= DateTime.Today);
                resend = resend.Where(t => t.InsertDate >= DateTime.Today);
                deliveryMail = deliveryMail.Where(t => t.InsertDate >= DateTime.Today);
                ticketsAssigned = ticketsAssigned.Where(t => t.InsertDate >= DateTime.Today);
                foreach (var day in agentActivities)
                {
                    var dailyActivities = new CallCenterWeekly
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
                foreach (var department in departmentActivities)
                {
                    var dailyActivitiesByDepartment = new CallCenterWeeklyByDepartment
                    {
                        Department = department.Key,
                        NumberSuccessfulCalls = successfulCalls.Count(a => a.Department == department.Key),
                        NumberUnsuccessfulCalls = unsuccessfulCalls.Count(a => a.Department == department.Key),
                        NumberMailchange = mailChange.Count(a => a.Department == department.Key),
                        NumberResend = resend.Count(a => a.Department == department.Key),
                        NumberMail = deliveryMail.Count(a => a.Department == department.Key)
                    };
                    activitiesByDepartment.Add(dailyActivitiesByDepartment);
                }
            }

            var model = new CallCenterWeeklyStatsViewModel
            {
                Activities = activities.AsQueryable(),
                ActivitiesByDepartment = activitiesByDepartment.AsQueryable(),
                SumSuccessfulCalls = successfulCalls.Count(),
                SumUnsuccessfulCalls = unsuccessfulCalls.Count(),
                SumMailchange = mailChange.Count(),
                SumResend = resend.Count(),
                SumSentMail = deliveryMail.Count(),
                SumTicketsAssigned = ticketsAssigned.Count()
            };

            return View(model);
        }

        // GET: Stats/Delivery
        public ActionResult Delivery(string search)
        {
            var createdTickets = from t in db.DeliveryTicketModels
                                  select t;
            var createdTicketsFirst = from t in db.DeliveryTicketModels
                                       where t.FirstInvoice
                                       select t;
            var groupedDeliveries = (from t in db.DeliveryTicketModels
                                     where t.InsertDate >= DateTime.Today
                                     group t by new { Date = DbFunctions.TruncateTime(t.SentDate), t.AssignedTo }  into gt
                                     select gt).ToList();
            var deliveries = new List<DailyDelivery>();

            if (!String.IsNullOrEmpty(search))
            {
                var searchDate = Convert.ToDateTime(search);
                var searchDatePlus = searchDate.AddDays(1);
                createdTickets = createdTickets.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                createdTicketsFirst = createdTicketsFirst.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                groupedDeliveries = (from t in db.DeliveryTicketModels
                                     where (t.InsertDate > searchDate) && (t.InsertDate < searchDatePlus)
                                     group t by new { Date = DbFunctions.TruncateTime(t.SentDate), t.AssignedTo } into gt
                                     select gt).ToList();

                foreach (var day in groupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)day.Key.Date,
                        CreatedTicketsCount = day.Count(),
                        CreatedTicketsFirstTimeCount = day.Count(t => t.FirstInvoice),
                        SentCount = day.Count(t => t.DocumentStatus == 30),
                        DeliveredCount = day.Count(t => t.DocumentStatus == 40),
                        OtherCount = day.Count(t => (t.DocumentStatus != 30 && t.DocumentStatus != 40)),
                        AssignedToCount = day.Count(t => t.IsAssigned),
                        AssignedTo = day.Key.AssignedTo
                    };
                    deliveries.Add(dailyDelivery);
                }
            }
            else
            {
                createdTickets = createdTickets.Where(t => t.InsertDate >= DateTime.Today);
                createdTicketsFirst = createdTicketsFirst.Where(t => t.InsertDate >= DateTime.Today);

                foreach (var day in groupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)day.Key.Date,
                        CreatedTicketsCount = day.Count(),
                        CreatedTicketsFirstTimeCount = day.Count(t => t.FirstInvoice),
                        SentCount = day.Count(t => t.DocumentStatus == 30),
                        DeliveredCount = day.Count(t => t.DocumentStatus == 40),
                        OtherCount = day.Count(t => (t.DocumentStatus != 30 && t.DocumentStatus != 40)),
                        AssignedToCount = day.Count(t => t.IsAssigned),
                        AssignedTo = day.Key.AssignedTo
                    };
                    deliveries.Add(dailyDelivery);
                }  
            }

            var model = new DeliveryStatsViewModel
            {
                CreatedTicketsTodayCount = createdTickets.Count(),
                CreatedTicketsTodayFirstTimeCount = createdTicketsFirst.Count(),
                Deliveries = deliveries.AsQueryable()
            };

            var date = new DateTime(2017, 7, 1);
            ViewBag.TotalOpenedTickets = (from t in db.DeliveryTicketModels
                                          where t.IsAssigned == false && t.InsertDate >= date && t.DocumentStatus == 30
                                          select t).Count();

            return View(model);
        }

         // GET: Stats/Sales
         public ActionResult SalesStat(string Agent, string SearchDateStart, string SearchDateEnd)
         {
 
 
             var agents = from u in db.Users
                            select u;
             var leads = from u in db.Leads
                          where u.IsAssigned
                          select u;
             var opportunities = from u in db.Opportunities
                                  where u.IsAssigned
                                  select u;
 
             var assignedOpportunities = db.Opportunities.Where(s => s.IsAssigned).GroupBy(d => d.AssignedTo)
                 .Select(d => new SaleAgentGrouping
                 {
                    Name = d.Key,
                    Count = d.Count()
                 });
 
             var assignedLeads = db.Leads.Where(s => s.IsAssigned).GroupBy(d => d.AssignedTo)
                .Select(d => new SaleAgentGrouping
                {
                    Name = d.Key,
                    Count = d.Count()
                });
 
 
             if(!String.IsNullOrEmpty(SearchDateStart) && !String.IsNullOrEmpty(SearchDateEnd))
             {
               /* 
              
               _Leads = _Leads.Where(t => t.InsertDate=>)
                  _Opportunities = 
 
                  assignedOpportunities = 
 
                  assignedLeads =
                  
              */
             }
             if (String.IsNullOrEmpty(SearchDateStart) && String.IsNullOrEmpty(SearchDateEnd))
             {
 
             }
             if (!String.IsNullOrEmpty(SearchDateStart) && String.IsNullOrEmpty(SearchDateEnd))
             {
 
             }
             if (String.IsNullOrEmpty(SearchDateStart) && !String.IsNullOrEmpty(SearchDateEnd))
             {
 
             }
 
             var salesStat = new SalesStatsViewModel
             {
                 Leads = leads,
                 Opportunities = opportunities,
                 SumAssignedOpportunities = assignedOpportunities,
                 SumAssignedLeads=assignedLeads,
                 Agents = agents,
             };
 
             return View(salesStat);
 
         }
    }
}