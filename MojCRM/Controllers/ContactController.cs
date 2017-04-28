using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
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

        // GET: Contact/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Organization,ContactFirstName,ContactLastName,Title,TelephoneNumber,MobilePhoneNumber,Email,User,InsertDate,UpdateDate,ContactType")] Contact newContact)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(newContact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: Contact/CreateFromDelivery
        [HttpPost]
        public ActionResult CreateFromDelivery(string FirstName, string LastName, string Telephone, string Mobile, string Email, string Agent, string Receiver, string DocumentId)
        {
            int ReceiverInt = Int32.Parse(Receiver);
            int DocumentIdInt = Int32.Parse(DocumentId);

            try
            {
                db.Contacts.Add(new Contact
                {
                    OrganizationId = ReceiverInt,
                    ContactFirstName = FirstName,
                    ContactLastName = LastName,
                    Title = "N/A",
                    TelephoneNumber = Telephone,
                    MobilePhoneNumber = Mobile,
                    Email = Email,
                    User = Agent,
                    InsertDate = DateTime.Now,
                    ContactType = "Delivery",
                });

                db.SaveChanges();

                return RedirectToAction("Details", "Delivery", new { id = DocumentIdInt, receiverId = ReceiverInt, Name = User.Identity.Name });
            }
            // TO DO: This catch part throws DbEntityValidationException in first foreach... I need to check why...
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    db.LogError.Add(new LogError
                    {
                        Method = @"Contact - CreateFromDelivery",
                        Parameters = @"Id = " + Receiver + @" FirstName = '" + FirstName + @"' LastName = '" + LastName + @"' TelephoneNumber = '" + Telephone + @"' MobilePhoneNumber = '" + Mobile + @"' Email = '" + Email + @"' User = '" + Agent + @"'",
                        Message = @"Entity of type '" + eve.Entry.Entity.GetType().Name.ToString() + @"' in state '" + eve.Entry.State.ToString() + @"' has following validation errors",
                        InnerException = "",
                        Request = "",
                        User = Agent,
                        InsertDate = DateTime.Now
                    });
                    db.SaveChanges();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        db.LogError.Add(new LogError
                        {
                            Method = @"Contact - CreateFromDelivery",
                            Parameters = "",
                            Message = "Property " + ve.PropertyName + " Error " + ve.ErrorMessage,
                            InnerException = "",
                            Request = "",
                            User = Agent,
                            InsertDate = DateTime.Now
                        });
                        db.SaveChanges();
                    }
                }
                throw;
            }
        }

        // POST: Contact/EditFromDelivery
        [HttpPost]
        public ActionResult EditFromDelivery(string _FirstName, string _LastName, string _Telephone, string _Mobile, string _Email, string _Agent, string _Receiver, string _DocumentId)
        {
            int _ReceiverInt = Int32.Parse(_Receiver);
            int _DocumentIdInt = Int32.Parse(_DocumentId);

            var ContactForUpdate = from c in db.Contacts
                                   where c.ContactType == "Delivery" && c.OrganizationId == _ReceiverInt
                                   select c;

            foreach (Contact c in ContactForUpdate)
            {
                c.ContactFirstName = _FirstName;
                c.ContactLastName = _LastName;
                c.TelephoneNumber = _Telephone;
                c.MobilePhoneNumber = _Mobile;
                c.Email = _Email;
                c.UpdateDate = DateTime.Now;
                c.User = _Agent;
            }
            db.SaveChanges();

            return RedirectToAction("Details", "Delivery", new { id = _DocumentIdInt, receiverId = _ReceiverInt, Name = User.Identity.Name });
        }

        public JsonResult GetOrganization(string term = "")
        {
            var OrganizationList = db.Organizations.Where(
                c => 
                    c.SubjectName.Contains(term) || 
                    c.VAT.Contains(term)
                    )
                    .Select(c => new { Naziv = c.SubjectName, OIB = c.VAT }).Distinct().ToList();
            return Json(OrganizationList, JsonRequestBehavior.AllowGet);
        }
    }
}