using MojCRM.Models;
using MojCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Controllers
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
        public ActionResult PersonalDailyActivities(string Name, string Agent)
        {
            var ReferenceDate = DateTime.Today.AddDays(-1);
            var _Agents = (from u in db.Users
                           select u).AsEnumerable();
            var _Activities = (from a in db.ActivityLogs
                               select a).ToList();

            if (!String.IsNullOrEmpty(Name))
            {
                _Activities = _Activities.Where(a => (a.User == Name) && (a.InsertDate >= ReferenceDate)).ToList();
                var _DistinctDepartments = (from a in db.ActivityLogs
                                            where (a.User == Name) && (a.InsertDate >= ReferenceDate)
                                            select a.Department).Distinct().Count();
                ViewBag.User = Name;
                ViewBag.DistinctDepartments = _DistinctDepartments;
            }
            else if (!String.IsNullOrEmpty(Agent))
            {
                _Activities = _Activities.Where(a => (a.User == Agent) && (a.InsertDate >= ReferenceDate)).ToList();
                var _DistinctDepartments = (from a in db.ActivityLogs
                                            where (a.User == Agent) && (a.InsertDate >= ReferenceDate)
                                            select a.Department).Distinct().Count();
                ViewBag.User = Agent;
                ViewBag.DistinctDepartments = _DistinctDepartments;
            }

            var PersonalActivities = new PersonalDailyActivitiesViewModel
            {
                PersonalActivities = _Activities,
                Agents = _Agents

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
                                where a.ActivityType == ActivityLog.ActivityTypeEnum.DELMAIL
                                select a);
            var Activities = new List<CallCenterDaily>();

            if (!String.IsNullOrEmpty(SearchDate))
            {
                var searchDate = Convert.ToDateTime(SearchDate);
                var searchDatePlus = searchDate.AddDays(1);
                SuccessfulCalls = SuccessfulCalls.Where(a => (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus));
                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                MailChange = MailChange.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                Resend = Resend.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                DeliveryMail = DeliveryMail.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                AgentActivities = (from a in db.ActivityLogs
                                   where (a.InsertDate >= searchDate) && (a.InsertDate < searchDatePlus)
                                   group a by a.User into ga
                                   select ga).ToList();
                foreach (var Day in AgentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = Day.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Count(),
                        NumberMailchange = MailChange.Count(),
                        NumberResend = Resend.Count(),
                        NumberDeliveryMail = DeliveryMail.Count()
                    };
                    Activities.Add(dailyActivities);
                }
            }
            else
            {
                SuccessfulCalls = SuccessfulCalls.Where(a => a.InsertDate >= DateTime.Today);
                UnsuccessfulCalls = UnsuccessfulCalls.Where(t => t.InsertDate >= DateTime.Today);
                MailChange = MailChange.Where(t => t.InsertDate >= DateTime.Today);
                Resend = Resend.Where(t => t.InsertDate >= DateTime.Today);
                DeliveryMail = DeliveryMail.Where(t => t.InsertDate >= DateTime.Today);
                foreach (var Day in AgentActivities)
                {
                    var dailyActivities = new CallCenterDaily
                    {
                        Agent = Day.Key,
                        NumberSuccessfulCalls = SuccessfulCalls.Count(),
                        NumberUnsuccessfulCalls = UnsuccessfulCalls.Count(),
                        NumberMailchange = MailChange.Count(),
                        NumberResend = Resend.Count(),
                        NumberDeliveryMail = DeliveryMail.Count()
                    };
                    Activities.Add(dailyActivities);
                }
            }

            var model = new CallCenterDailyStatsViewModel
            {
                Activities = Activities
            };

            return View(model);
        }

        // GET: Stats/Delivery
        public ActionResult Delivery(string SearchDate)
        {
            var CreatedTickets = (from t in db.DeliveryTicketModels
                                  select t);
            var CreatedTicketsFirst = (from t in db.DeliveryTicketModels
                                       where (t.FirstInvoice == true)
                                       select t);
            var GroupedDeliveries = (from t in db.DeliveryTicketModels
                                     where t.InsertDate >= DateTime.Today
                                     group t by DbFunctions.TruncateTime(t.SentDate) into gt
                                     select gt).ToList();
            var Deliveries = new List<DailyDelivery>();

            if (!String.IsNullOrEmpty(SearchDate))
            {
                var searchDate = Convert.ToDateTime(SearchDate);
                var searchDatePlus = searchDate.AddDays(1);
                CreatedTickets = CreatedTickets.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                CreatedTicketsFirst = CreatedTicketsFirst.Where(t => (t.InsertDate >= searchDate) && (t.InsertDate < searchDatePlus));
                GroupedDeliveries = (from t in db.DeliveryTicketModels
                                     where (t.InsertDate > searchDate) && (t.InsertDate < searchDatePlus)
                                     group t by DbFunctions.TruncateTime(t.SentDate) into gt
                                     select gt).ToList();
                foreach (var Day in GroupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)Day.Key,
                        CreatedTicketsCount = Day.Count(),
                        CreatedTicketsFirstTimeCount = Day.Where(t => t.FirstInvoice == true).Count()
                    };
                    Deliveries.Add(dailyDelivery);
                }
            }
            else
            {
                CreatedTickets = CreatedTickets.Where(t => t.InsertDate >= DateTime.Today);
                CreatedTicketsFirst = CreatedTicketsFirst.Where(t => t.InsertDate >= DateTime.Today);
                foreach (var Day in GroupedDeliveries)
                {
                    var dailyDelivery = new DailyDelivery
                    {
                        ReferenceDate = (DateTime)Day.Key,
                        CreatedTicketsCount = Day.Count(),
                        CreatedTicketsFirstTimeCount = Day.Where(t => t.FirstInvoice == true).Count()
                    };
                    Deliveries.Add(dailyDelivery);
                }
            }

            var model = new DeliveryStatsViewModel
            {
                CreatedTicketsTodayCount = CreatedTickets.Count(),
                CreatedTicketsTodayFirstTimeCount = CreatedTicketsFirst.Count(),
                Deliveries = Deliveries
            };

            return View(model);
        }
    }
}