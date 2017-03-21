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
        public ActionResult CreateFromDelivery(string FirstName, string LastName, string Telephone, string Mobile, string Email, string Agent, string Receiver)
        {
            var NewContact = new Contact();

            NewContact.ContactFirstName = FirstName;
            NewContact.ContactLastName = LastName;
            NewContact.ContactType = "Delivery";
            NewContact.TelephoneNumber = Telephone;
            NewContact.MobilePhoneNumber = Mobile;
            NewContact.Email = Email;
            NewContact.User.UserName = Agent;
            NewContact.InsertDate = DateTime.Now;
            NewContact.Organization.MerId = Int32.Parse(Receiver);

            return RedirectToAction("Details");
        }
    }
}