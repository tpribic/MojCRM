using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MojCRM.Models;
using MojCRM.ViewModels;
using ActiveUp.Net.Mail;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MojCRM.Helpers;
using PagedList;
using System.Data.Entity.Infrastructure;

namespace MojCRM.Controllers
{
    public class DeliveryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Delivery
        [Authorize]
        public async Task<ActionResult> Index(string SortOrder, string Sender, string Receiver, string InvoiceNumber, 
                                               string SentDate, string TicketDate/*, string DocumentType*/)
        {
            ViewBag.InsertDateParm = String.IsNullOrEmpty(SortOrder) ? "InsertDate" : "";

            var Results = from t in db.DeliveryTicketModels
                          where t.DocumentStatus == 30
                          select t;

            ViewBag.OpenTickets = Results.Count();

            if (!String.IsNullOrEmpty(Sender))
            {
                Results = Results.Where(t => t.Sender.SubjectName.Contains(Sender));
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(Receiver))
            {
                Results = Results.Where(t => t.Receiver.SubjectName.Contains(Receiver));
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(InvoiceNumber))
            {
                Results = Results.Where(t => t.InvoiceNumber.Contains(InvoiceNumber));
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(SentDate))
            {
                var sentDate = Convert.ToDateTime(SentDate);
                Results = Results.Where(t => t.SentDate == sentDate);
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(TicketDate))
            {
                var insertDate = Convert.ToDateTime(TicketDate);
                Results = Results.Where(t => t.InsertDate > insertDate);
                ViewBag.SearchResults = Results.Count();
            }

            //if (!String.IsNullOrEmpty(DocumentType))
            //{
            //    var ResultsDocumentType = from t in db.DeliveryTicketModels
            //                              where t.MerDocumentTypeIdString == DocumentType
            //                              select t.MerDocumentTypeId;
            //    ViewBag.SearchResults = Results.Count();
            //}

            switch (SortOrder)
            {
                case "InsertDate":
                    Results = Results.OrderBy(d => d.InsertDate);
                    break;
                default:
                    Results = Results.OrderByDescending(d => d.InsertDate);
                    break;
            }

            return View(await Results.ToListAsync());
        }

        // GET : Delivery/CreateTickets
        public ActionResult CreateTickets()
        {
            using (Imap4Client imap = new Imap4Client())
            {
                imap.Connect("mail.moj-eracun.hr");
                imap.Login("dostava@moj-eracun.hr", "m0j.d05tava");
                Mailbox inbox = imap.SelectMailbox("Inbox");

                string MerPattern1 = ".*?";
                string MerPattern2 = "((?:http|https)(?::\\/{2}[\\w]+)(?:[\\/|\\.]?)(?:[^\\s\"]*))";
                Regex MerLink = new Regex(MerPattern1 + MerPattern2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string MerCheck1 = "(Return)(-)(Path)(:)( )(dostava@moj-eracun\\.hr)";
                Regex MerChecker = new Regex(MerCheck1, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                int OrdinalNumber = inbox.MessageCount;
                int ReferenceOrdinalNumber = 96600;
                DateTime ReferenceDate = DateTime.Today;
                string DeliveryLink;
                DateTime Date = DateTime.Parse(inbox.Fetch.InternalDate(OrdinalNumber));

                while (Date > ReferenceDate)
                {
                    for (OrdinalNumber = inbox.MessageCount; Date > ReferenceDate && OrdinalNumber > ReferenceOrdinalNumber; OrdinalNumber--)
                    {
                        string Body = inbox.Fetch.MessageString(OrdinalNumber);
                        try
                        {
                            Match MerLinkMatch = MerLink.Match(Body);
                            Match MerCheckerMatch = MerChecker.Match(Body);
                            if (MerLinkMatch.Success && MerCheckerMatch.Success)
                            {
                                DeliveryLink = MerLinkMatch.Groups[1].ToString();
                                MerDeliveryJsonResponse Result = ParseJson(DeliveryLink);

                                    db.DeliveryTicketModels.Add(new Delivery
                                    {
                                        SenderId = Int32.Parse(Result.SupplierID),
                                        ReceiverId = Int32.Parse(Result.BuyerID),
                                        InvoiceNumber = Result.InterniBroj,
                                        MerLink = DeliveryLink,
                                        MerJson = Result,
                                        MerElectronicId = Result.Id,
                                        SentDate = Result.IssueDate,
                                        MerDocumentTypeId = Result.Type,
                                        DocumentStatus = Result.Status,
                                        InsertDate = DateTime.Now,
                                        BuyerEmail = Result.EmailPrimatelja,
                                    });
                                    db.SaveChanges();
                                }
                            //TO DO: Remove else and add it to catch segment
                            else
                            {
                                db.LogError.Add(new LogError
                                {
                                    Method = @"Delivery - CreateTickets",
                                    Parameters = "",
                                    Message = "Moj-CRM was unable to generete ticket",
                                    InnerException = "",
                                    Request = "",
                                    User = "Moj-CRM",
                                    InsertDate = DateTime.Now,
                                });
                            }
                            }
                        //TO DO: Add Exceptions
                        catch
                        {

                            throw;
                        }
                        Date = DateTime.Parse(inbox.Fetch.InternalDate(OrdinalNumber));
                    }
                }
                imap.Disconnect();
            }

            var Count = (from t in db.DeliveryTicketModels
                         where t.InsertDate > DateTime.Today
                         select t).Count();
            ViewBag.Count = Count;

            return View();
        }

        //GET: Delivery/UpdateAllStatuses
        public ActionResult UpdateAllStatuses()
        {
            var OpenTickets = from t in db.DeliveryTicketModels
                              where t.DocumentStatus == 30
                              select t.MerLink;

            var MerLinks = OpenTickets.ToList();

            foreach (var Link in MerLinks)
            {
                    MerDeliveryJsonResponse Result = ParseJson(Link);

                    var TicketForUpdate = from t in db.DeliveryTicketModels
                                          where t.MerLink == Link
                                          select t;
                    foreach (Delivery t in TicketForUpdate)
                    {
                        t.DocumentStatus = Result.Status;
                        t.BuyerEmail = Result.EmailPrimatelja;
                        t.UpdateDate = DateTime.Now;
                    }
                        db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST Delivery/Remove/1125768
        [HttpPost]
        public JsonResult Remove(int MerElectronicId)
        {
            var Tickets = from t in db.DeliveryTicketModels
                                   where t.MerElectronicId == MerElectronicId
                                   select t;
            var TicketForRemoval = Tickets.ToList().First();

            TicketForRemoval.DocumentStatus = 55;
            TicketForRemoval.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // POST Delivery/RemoveAll
        [HttpPost]
        public JsonResult RemoveAll(int[] MerElectronicIds)
        {
            foreach (var Id in MerElectronicIds)
            {
                var Tickets = from t in db.DeliveryTicketModels
                              where t.MerElectronicId == Id
                              select t;
                var TicketForRemoval = Tickets.ToList().First();

                TicketForRemoval.DocumentStatus = 55;
                TicketForRemoval.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }

            return Json(new { Status = "OK" });
        }

        // GET: Delivery/Details/5
        [Authorize]
        public ActionResult Details(int? id, int? receiverId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery deliveryTicketModel = db.DeliveryTicketModels.Find(id);
            if (deliveryTicketModel == null)
            {
                return HttpNotFound();
            }

            var _UndeliveredInvoicesList = (from t in db.DeliveryTicketModels
                                           where t.Id != id && t.DocumentStatus == 30 && t.ReceiverId == deliveryTicketModel.ReceiverId
                                           select t).ToList();

            var _RelatedDeliveryContacts = (from t in db.Contacts
                                            where t.Organization.MerId == receiverId && t.ContactType == "Delivery"
                                            select t).ToList();

            var _RelatedDeliveryDetails = (from t in db.DeliveryDetails
                                           where t.Receiver.MerId == receiverId
                                           select t).ToList();

            var DeliveryDetails = new DeliveryDetailsViewModel
            {
                TicketId = deliveryTicketModel.Id,
                SenderName = deliveryTicketModel.Sender.SubjectName,
                ReceiverName = deliveryTicketModel.Receiver.SubjectName,
                InvoiceNumber = deliveryTicketModel.InvoiceNumber,
                SentDate = deliveryTicketModel.SentDate,
                MerDocumentTypeId = deliveryTicketModel.MerDocumentTypeId,
                ReceiverEmail = deliveryTicketModel.BuyerEmail,
                MerDeliveryDetailComment = deliveryTicketModel.Receiver.MerDeliveryDetail.Comments,
                MerDeliveryDetailTelephone = deliveryTicketModel.Receiver.MerDeliveryDetail.Telephone,
                ReceiverId = deliveryTicketModel.ReceiverId,
                UndeliveredInvoices = _UndeliveredInvoicesList,
                RelatedDeliveryContacts = _RelatedDeliveryContacts,
                RelatedDeliveryDetails = _RelatedDeliveryDetails
            };

            var _InvoiceNumber = from t in db.DeliveryTicketModels
                                 where t.Id == id
                                 select t.InvoiceNumber;

            ViewBag.InvoiceNumber = _InvoiceNumber.First();

            

            return View(DeliveryDetails);
        }

        // GET: Delivery/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddDetail/1125768
        [HttpPost]
        public ActionResult AddDetail(int _ReceiverId, string _Agent, string _DetailNote, string _ContactId, int _TicketId)
        {
            try
            {
                db.DeliveryDetails.Add(new DeliveryDetail
                {
                    ReceiverId = _ReceiverId,
                    User = _Agent,
                    DetailNote = _DetailNote,
                    InsertDate = DateTime.Now,
                    Contact = _ContactId,
                    TicketId = _TicketId
                });
                db.SaveChanges();
            }
            catch (NullReferenceException ex)
            {
                db.LogError.Add(new LogError
                {
                    Method = @"Delivery - AddDetail",
                    Parameters = ex.Source,
                    Message = ex.Message,
                    InnerException = "",
                    Request = ex.TargetSite.ToString(),
                    User = _Agent,
                    InsertDate = DateTime.Now
                });
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                db.LogError.Add(new LogError
                {
                    Method = @"Delivery - AddDetail",
                    Parameters = ex.Source,
                    Message = ex.Message,
                    InnerException = ex.InnerException.ToString(),
                    Request = ex.TargetSite.ToString(),
                    User = _Agent,
                    InsertDate = DateTime.Now
                });
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = _TicketId, receiverId = _ReceiverId });
        }

        // POST: Delivery/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,SenderId,RecepientId,InvoiceNumber,UserId,MeRLink,InsertDate,UpdateDate")] Delivery deliveryTicketModel)
        {
            if (ModelState.IsValid)
            {
                db.DeliveryTicketModels.Add(deliveryTicketModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(deliveryTicketModel);
        }

        // GET: Delivery/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery deliveryTicketModel = await db.DeliveryTicketModels.FindAsync(id);
            if (deliveryTicketModel == null)
            {
                return HttpNotFound();
            }
            return View(deliveryTicketModel);
        }

        // POST: Delivery/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SenderId,RecepientId,InvoiceNumber,UserId,MeRLink,InsertDate,UpdateDate")] Delivery deliveryTicketModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deliveryTicketModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(deliveryTicketModel);
        }

        // GET: Delivery/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delivery deliveryTicketModel = await db.DeliveryTicketModels.FindAsync(id);
            if (deliveryTicketModel == null)
            {
                return HttpNotFound();
            }
            return View(deliveryTicketModel);
        }

        // POST: Delivery/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Delivery deliveryTicketModel = await db.DeliveryTicketModels.FindAsync(id);
            db.DeliveryTicketModels.Remove(deliveryTicketModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        static MerDeliveryJsonResponse ParseJson(string url)
        {
            // TO DO: Make try-catch for the entire method
            using (var MeR = new WebClient())
            {
                var json = MeR.DownloadString(url);
                MerDeliveryJsonResponse Result = JsonConvert.DeserializeObject<MerDeliveryJsonResponse>(json);
                return (Result);
            }
        }
    }
}
