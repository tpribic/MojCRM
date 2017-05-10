using MojCRM.Models;
using MojCRM.ViewModels;
using System;
using System.Collections.Generic;
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
                _Activities = _Activities.Where(a => a.User == Name && a.InsertDate == ReferenceDate).ToList();
                ViewBag.User = Name;
            }
            else if (!String.IsNullOrEmpty(Agent))
            {
                _Activities = _Activities.Where(a => a.User == Agent && a.InsertDate == ReferenceDate).ToList();
                ViewBag.User = Agent;
            }

            var PersonalActivities = new PersonalDailyActivitiesViewModel
            {
                PersonalActivities = _Activities,
                Agents = _Agents
            };

            var _DistinctDepartments = (from a in db.ActivityLogs
                                        where a.InsertDate == ReferenceDate
                                        select a.Department).Distinct().Count();
            ViewBag.DistinctDepartments = _DistinctDepartments;

            return View(PersonalActivities);
        }

        // GET: Stats/CallCenterDaily
        public ActionResult CallCenterDaily()
        {
            //var Results = (from a in db.ActivityLogs
            //               where a.InsertDate == DateTime.Today
            //               select a).GroupBy(a => a.User).ToList();

            //var Activities = new CallCenterDailyStatsViewModel
            //{
            //    Activities = Results,
            //};

            return View();
        }

        // GET: Stats/Delivery
        public ActionResult Delivery(string SearchDate)
        {
            var CreatedTickets = (from t in db.DeliveryTicketModels
                                  where t.InsertDate == DateTime.Today
                                  select t);
            var CreatedTicketsFirst = (from t in db.DeliveryTicketModels
                                       where t.InsertDate == DateTime.Today && t.FirstInvoice == true
                                       select t);
            ViewBag.CreatedTickets = CreatedTickets.Count();
            ViewBag.CreatedTicketsFirst = CreatedTicketsFirst.Count();

            if (!String.IsNullOrEmpty(SearchDate))
            {
                var searchDate = Convert.ToDateTime(SearchDate);
                CreatedTickets = CreatedTickets.Where(t => t.InsertDate == searchDate);
                CreatedTicketsFirst = CreatedTicketsFirst.Where(t => t.InsertDate == searchDate);
                ViewBag.CreatedTickets = CreatedTickets.Count();
                ViewBag.CreatedTicketsFirst = CreatedTicketsFirst.Count();
            }
            return View();
        }
    }
}