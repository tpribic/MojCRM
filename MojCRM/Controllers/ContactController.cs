using MojCRM.Areas.HelpDesk.Helpers;
using MojCRM.Areas.Sales.Helpers;
using MojCRM.Helpers;
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
            var contacts = from c in db.Contacts
                           select c;
            return View(contacts.ToList());
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
        public ActionResult CreateFromDelivery(DeliveryContactHelper model)
        {
            try
            {
                db.Contacts.Add(new Contact
                {
                    OrganizationId = model.ReceiverId,
                    ContactFirstName = model.FirstName,
                    ContactLastName = model.LastName,
                    Title = model.TitleFunction,
                    TelephoneNumber = model.Telephone,
                    MobilePhoneNumber = model.Mobile,
                    Email = model.Email,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now,
                    ContactType = Contact.ContactTypeEnum.DELIVERY,
                });

                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
            }
            // TO DO: This catch part throws DbEntityValidationException in first foreach... I need to check why...
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    db.LogError.Add(new LogError
                    {
                        Method = @"Contact - CreateFromDelivery",
                        Parameters = @"Id = " + model.ReceiverId + @" FirstName = '" + model.FirstName + @"' LastName = '" + model.LastName + @"' TelephoneNumber = '" + model.Telephone + @"' MobilePhoneNumber = '" + model.Mobile + @"' Email = '" + model.Email + @"' User = '" + User.Identity.Name + @"'",
                        Message = @"Entity of type '" + eve.Entry.Entity.GetType().Name.ToString() + @"' in state '" + eve.Entry.State.ToString() + @"' has following validation errors",
                        InnerException = "",
                        Request = "",
                        User = User.Identity.Name,
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
                            User = User.Identity.Name,
                            InsertDate = DateTime.Now
                        });
                        db.SaveChanges();
                    }
                }
                throw;
            }
        }

        // POST: Contact/CreateFromDelivery
        [HttpPost]
        public ActionResult CreateFromSales(SalesContactHelper Model)
        {
            var _OrganizationId = (from o in db.Opportunities
                                   where o.OpportunityId == Model.RelatedEntityId
                                   select o.RelatedOrganizationId).First().ToString();

            db.Contacts.Add(new Contact
            {
                OrganizationId = Int32.Parse(_OrganizationId),
                ContactFirstName = Model.FirstName,
                ContactLastName = Model.LastName,
                Title = Model.TitleFunction,
                TelephoneNumber = Model.Telephone,
                MobilePhoneNumber = Model.Mobile,
                Email = Model.ContactEmail,
                User = User.Identity.Name,
                InsertDate = DateTime.Now,
                ContactType = Contact.ContactTypeEnum.SALES,
            });

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Contact/CreateFromDelivery
        [HttpPost]
        public ActionResult CreateFromSalesLead(SalesContactHelper Model)
        {
            var _OrganizationId = (from o in db.Leads
                                   where o.LeadId == Model.RelatedEntityId
                                   select o.RelatedOrganizationId).First().ToString();

            db.Contacts.Add(new Contact
            {
                OrganizationId = Int32.Parse(_OrganizationId),
                ContactFirstName = Model.FirstName,
                ContactLastName = Model.LastName,
                Title = Model.TitleFunction,
                TelephoneNumber = Model.Telephone,
                MobilePhoneNumber = Model.Mobile,
                Email = Model.ContactEmail,
                User = User.Identity.Name,
                InsertDate = DateTime.Now,
                ContactType = Contact.ContactTypeEnum.SALES,
            });

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
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
        public ActionResult EditFromDelivery(DeliveryContactHelper model)
        {
            var ContactForUpdate = db.Contacts.Find(model.ContactId);

            if (!String.IsNullOrEmpty(model.FirstName))
                ContactForUpdate.ContactFirstName = model.FirstName;
            if (!String.IsNullOrEmpty(model.LastName))
                ContactForUpdate.ContactLastName = model.LastName;
            if (!String.IsNullOrEmpty(model.Telephone))
                ContactForUpdate.TelephoneNumber = model.Telephone;
            if (!String.IsNullOrEmpty(model.Mobile))
                ContactForUpdate.MobilePhoneNumber = model.Mobile;
            if (!String.IsNullOrEmpty(model.Email))
                ContactForUpdate.Email = model.Email;
            if (!String.IsNullOrEmpty(model.TitleFunction))
                ContactForUpdate.Title = model.TitleFunction;

            ContactForUpdate.UpdateDate = DateTime.Now;
            ContactForUpdate.User = User.Identity.Name;
            db.SaveChanges();

            return RedirectToAction("Details", "Delivery", new { area = "HelpDesk", id = model.TicketId, receiverId = model.ReceiverId });
        }

        // POST: Contact/EditFromSales
        [HttpPost]
        public ActionResult EditFromSales(SalesContactHelper Model)
        {
            var contactId = Int32.Parse(Model.ContactId);
            var ContactForUpdate = db.Contacts.Find(contactId);

            if (!String.IsNullOrEmpty(Model.FirstName))
            {
                ContactForUpdate.ContactFirstName = Model.FirstName;
            }
            if (!String.IsNullOrEmpty(Model.LastName))
            {
                ContactForUpdate.ContactLastName = Model.LastName;
            }
            if (!String.IsNullOrEmpty(Model.Telephone))
            {
                ContactForUpdate.TelephoneNumber = Model.Telephone;
            }
            if (!String.IsNullOrEmpty(Model.Mobile))
            {
                ContactForUpdate.MobilePhoneNumber = Model.Mobile;
            }
            if (!String.IsNullOrEmpty(Model.ContactEmail))
            {
                ContactForUpdate.Email = Model.ContactEmail;
            }
            if (!String.IsNullOrEmpty(Model.TitleFunction))
            {
                ContactForUpdate.Title = Model.TitleFunction;
            }
            ContactForUpdate.User = User.Identity.Name;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        //// POST: Contact/EditFromSales
        //[HttpPost]
        //public ActionResult EditFromSalesLead(LeadContactHelper Model)
        //{
        //    var ContactForUpdate = (from c in db.Contacts
        //                            where (c.ContactFirstName + " " + c.ContactLastName) == Model.ContactId.ToString()
        //                            select c).First();

        //    if (!String.IsNullOrEmpty(Model.FirstName))
        //    {
        //        ContactForUpdate.ContactFirstName = Model.FirstName;
        //    }
        //    if (!String.IsNullOrEmpty(Model.LastName))
        //    {
        //        ContactForUpdate.ContactLastName = Model.LastName;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Telephone))
        //    {
        //        ContactForUpdate.TelephoneNumber = Model.Telephone;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Mobile))
        //    {
        //        ContactForUpdate.MobilePhoneNumber = Model.Mobile;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Email))
        //    {
        //        ContactForUpdate.Email = Model.Email;
        //    }
        //    if (!String.IsNullOrEmpty(Model.Title))
        //    {
        //        ContactForUpdate.Title = Model.Title;
        //    }
        //    ContactForUpdate.User = User.Identity.Name;
        //    db.SaveChanges();

        //    return Redirect(Request.UrlReferrer.ToString());
        //}

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