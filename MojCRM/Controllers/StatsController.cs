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

        // GET: Stats/CallCenterDaily
        public ActionResult CallCenterDaily()
        {
            var Results = (from a in db.ActivityLogs
                           group a by a.ActivityType into al
                           select new { ActivityKey = al.Key, ActivityType = al.ToList()});
            return View(Results);
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