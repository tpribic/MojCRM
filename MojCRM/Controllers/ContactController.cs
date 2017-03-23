using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Controllers
{
    public class ContactController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        // GET: Contact/CreateFromDelivery
        public ActionResult CreateFromDelivery(string FirstName, string LastName, string Telephone, string Mobile, string Email, string Agent, string Receiver, string DocumentId)
        {
            int ReceiverInt = Int32.Parse(Receiver);

            db.Contacts.Add(new Contact
            {
                Organization = new Organizations { MerId = ReceiverInt },
                ContactFirstName = FirstName,
                ContactLastName = LastName,
                Title = "N/A",
                TelephoneNumber = Telephone,
                MobilePhoneNumber = Mobile,
                Email = Email,
                User = new ApplicationUser { UserName = Agent },
                InsertDate = DateTime.Now,
                ContactType = "Delivery",
            });

            db.SaveChanges();

            return RedirectToAction("Index", "Delivery");
        }
    }
}