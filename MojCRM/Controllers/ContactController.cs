using MojCRM.Models;
using MojCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
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
                    ContactType = Contact.ContactTypeEnum.DELIVERY,
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

        // GET: Contact/Details
        public ActionResult Details(int ContactId)
        {
            if (ContactId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contactModel = db.Contacts.Find(ContactId);
            if (contactModel == null)
            {
                return HttpNotFound();
            }

            var _DeliveryDetails = (from d in db.DeliveryDetails
                                    where d.Contact == (contactModel.ContactFirstName + " " + contactModel.ContactLastName)
                                    select d).AsEnumerable();

            var ContactDetails = new ContactDetailsViewModel
            {
                ContactId = contactModel.ContactId,
                ContactFirstName = contactModel.ContactFirstName,
                ContactLastName = contactModel.ContactLastName,
                Title = contactModel.Title,
                TelephoneNumber = contactModel.TelephoneNumber,
                MobilePhoneNumber = contactModel.MobilePhoneNumber,
                Email = contactModel.Email,
                User = contactModel.User,
                InsertDate = contactModel.InsertDate,
                UpdateDate = contactModel.UpdateDate,
                ContactType = contactModel.ContactType,
                DeliveryDetails = _DeliveryDetails
            };

            return View(ContactDetails);
        }

        // GET: Coontact/Delete/5
        public ActionResult Delete(int? ContactId)
        {
            if (ContactId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(ContactId);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ContactId)
        {
            Contact contact = db.Contacts.Find(ContactId);
            db.Contacts.Remove(contact);
            db.SaveChanges();
            return View("Index");
        }

        // GET: Contact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact Contact = db.Contacts.Find(id);
            if (Contact == null)
            {
                return HttpNotFound();
            }
            return View(Contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContactId,ContactFirstName,ContactLastName,Title,TelephoneNumber,MobilePhoneNumber,Email,User,Agent,ContactType")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                var editedContact = (from c in db.Contacts
                                     where c.ContactId == contact.ContactId
                                     select c).First();
                editedContact.ContactFirstName = contact.ContactFirstName;
                editedContact.ContactLastName = contact.ContactLastName;
                editedContact.Title = contact.Title;
                editedContact.TelephoneNumber = contact.TelephoneNumber;
                editedContact.MobilePhoneNumber = contact.MobilePhoneNumber;
                editedContact.Email = contact.Email;
                editedContact.User = contact.User;
                editedContact.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Contacts", "Delivery");
            }
            return View(contact);
        }

        // POST: Contact/EditFromDelivery
        [HttpPost]
        public ActionResult EditFromDelivery(string _FirstName, string _LastName, string _Telephone, string _Mobile, string _Email, string _Agent, string _Receiver, string _DocumentId)
        {
            int _ReceiverInt = Int32.Parse(_Receiver);
            int _DocumentIdInt = Int32.Parse(_DocumentId);

            var ContactForUpdate = from c in db.Contacts
                                   where c.ContactType == Contact.ContactTypeEnum.DELIVERY && c.OrganizationId == _ReceiverInt
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