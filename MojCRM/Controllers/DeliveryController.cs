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
using System.Text;

namespace MojCRM.Controllers
{
    public class DeliveryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Delivery
        [Authorize]
        public async Task<ActionResult> Index(string SortOrder, string Sender, string Receiver, string InvoiceNumber, 
                                               string SentDate, string TicketDate, string BuyerEmail, string DocumentStatus, string DocumentType)
        {
            ViewBag.InsertDateParm = String.IsNullOrEmpty(SortOrder) ? "InsertDate" : String.Empty;

            var Results = from t in db.DeliveryTicketModels
                          where t.DocumentStatus == 30
                          select t;

            var ResultsAll = from t in db.DeliveryTicketModels
                          select t;

            ViewBag.OpenTickets = Results.Count();

            int DocumentStatusInt;
            if (!String.IsNullOrEmpty(DocumentStatus))
            {
                DocumentStatusInt = Int32.Parse(DocumentStatus);
            }
            else
            {
                DocumentStatusInt = 30;
            };

            int DocumentTypeInt;
            if (!String.IsNullOrEmpty(DocumentType))
            {
                DocumentTypeInt = Int32.Parse(DocumentType);
            }
            else
            {
                DocumentTypeInt = 1;
            };

            if (!String.IsNullOrEmpty(Sender))
            {
                Results = ResultsAll.Where(t => t.Sender.SubjectName.Contains(Sender) || t.Sender.VAT.Contains(Sender));
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(Receiver))
            {
                Results = ResultsAll.Where(t => t.Receiver.SubjectName.Contains(Receiver) || t.Receiver.VAT.Contains(Receiver));
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(InvoiceNumber))
            {
                Results = ResultsAll.Where(t => t.InvoiceNumber.Contains(InvoiceNumber));
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(SentDate))
            {
                var sentDate = Convert.ToDateTime(SentDate);
                Results = ResultsAll.Where(t => t.SentDate == sentDate);
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(TicketDate))
            {
                var insertDate = Convert.ToDateTime(TicketDate);
                Results = ResultsAll.Where(t => t.InsertDate > insertDate);
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(BuyerEmail))
            {
                Results = ResultsAll.Where(t => t.BuyerEmail.Contains(BuyerEmail));
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(DocumentStatus))
            {
                Results = ResultsAll.Where(t => t.DocumentStatus == DocumentStatusInt);
                ViewBag.SearchResults = Results.Count();
            }

            if (!String.IsNullOrEmpty(DocumentType))
            {
                Results = ResultsAll.Where(t => t.MerDocumentTypeId == DocumentTypeInt);
                ViewBag.SearchResults = Results.Count();
            }

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

        // GET: Delivery/CreateTickets
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
                                    Parameters = String.Empty,
                                    Message = "Moj-CRM was unable to generete ticket",
                                    InnerException = String.Empty,
                                    Request = String.Empty,
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

        // GET: Delivery/UpdateStatus/12345
        public ActionResult UpdateStatusIndex(int Id)
        {
            var OpenTicket = from t in db.DeliveryTicketModels
                              where t.Id == Id
                              select t.MerLink;

            var MerLinks = OpenTicket.ToList();

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

        public ActionResult UpdateStatus(int TicketId, int ReceiverId)
        {
            var OpenTicket = from t in db.DeliveryTicketModels
                             where t.Id == TicketId
                             select t.MerLink;

            var MerLinks = OpenTicket.ToList();

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

            return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId });
        }

        // GET: Delivery/UpdateAllStatuses
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

        // GET: Delivery/Resend/12345
        [Authorize]
        public ActionResult Resend(int? TicketId, int MerElectronicId, int? ReceiverId, string Name)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();

            MerApiResend Request = new MerApiResend();

            Request.Id = MerUser.ToString();
            Request.Pass = MerPass.ToString();
            Request.Oib = "99999999927";
            Request.PJ = "";
            Request.SoftwareId = "MojCRM-001";
            Request.DocumentId = MerElectronicId;

            string MerRequest = JsonConvert.SerializeObject(Request);

            using (var Mer = new WebClient())
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/changeEmail").ToString(), "POST", MerRequest);
            }

            if (TicketId == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId });
            }
        }

        // GET Delivery/ChangeEmail
        [Authorize]
        public ActionResult ChangeEmail(string _Id, string _Receiver,  string _Email, string _Agent, string _TicketId)
        {
            int _IdInt = Int32.Parse(_Id);
            int _TicketIdInt = Int32.Parse(_TicketId);
            int _ReceiverInt = Int32.Parse(_Receiver);

            var MerUser = (from u in db.Users
                          where u.UserName == _Agent
                          select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                          where u.UserName == _Agent
                          select u.MerUserPassword).First();

            MerApiChangeEmail Request = new MerApiChangeEmail();

            Request.Id = MerUser.ToString();
            Request.Pass = MerPass.ToString();
            Request.Oib = "99999999927";
            Request.PJ = "";
            Request.SoftwareId = "MojCRM-001";
            Request.DocumentId = _IdInt;
            Request.Email = _Email;

            string MerRequest = JsonConvert.SerializeObject(Request);

            using (var Mer = new WebClient())
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/changeEmail").ToString(), "POST", MerRequest);
            }

            UpdateStatus(_IdInt, _ReceiverInt);

            return RedirectToAction("Details", new { id = _TicketIdInt, receiverId = _ReceiverInt });
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

            var _RelatedInvoicesList = (from t in db.DeliveryTicketModels
                                        where t.Id != id && t.ReceiverId == deliveryTicketModel.ReceiverId && t.SentDate == DateTime.Now.AddMonths(-2)
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
                MerDocumentStatusId = deliveryTicketModel.DocumentStatus,
                ReceiverEmail = deliveryTicketModel.BuyerEmail,
                MerDeliveryDetailComment = deliveryTicketModel.Receiver.MerDeliveryDetail.Comments,
                MerDeliveryDetailTelephone = deliveryTicketModel.Receiver.MerDeliveryDetail.Telephone,
                MerElectronicId = deliveryTicketModel.MerElectronicId,
                ReceiverId = deliveryTicketModel.ReceiverId,
                ReceiverVAT = deliveryTicketModel.Receiver.VAT,
                SenderVAT = deliveryTicketModel.Sender.VAT,
                RelatedInvoices = _RelatedInvoicesList,
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
        public ActionResult AddDetail(int _ReceiverId, string _Agent, string _ContactId, string _DetailTemplate, string _DetailNote, int _TicketId)
        {
            if (_DetailTemplate == String.Empty && _DetailNote != String.Empty)
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
                        InnerException = String.Empty,
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
            }
            else if (_DetailNote == String.Empty && _DetailTemplate != String.Empty)
            {
                db.DeliveryDetails.Add(new DeliveryDetail
                {
                    ReceiverId = _ReceiverId,
                    User = _Agent,
                    DetailNote = _DetailTemplate,
                    InsertDate = DateTime.Now,
                    Contact = _ContactId,
                    TicketId = _TicketId
                });
                db.SaveChanges();
            }
            else
            {
                db.DeliveryDetails.Add(new DeliveryDetail
                {
                    ReceiverId = _ReceiverId,
                    User = _Agent,
                    DetailNote = _DetailTemplate + " - " + _DetailNote,
                    InsertDate = DateTime.Now,
                    Contact = _ContactId,
                    TicketId = _TicketId
                });
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = _TicketId, receiverId = _ReceiverId });
        }

        // POST: Delivery/EditDetail/12345
        [HttpPost]
        [Authorize]
        public ActionResult EditDetail(int _ReceiverId, string _Agent, string _ContactId, string _DetailNote, string _TicketId)
        {
            int _TicketIdInt = Int32.Parse(_TicketId);
            var DetailForEdit = from t in db.DeliveryDetails
                                 where t.ReceiverId == _ReceiverId && t.TicketId == _TicketIdInt
                                 select t;

            foreach (DeliveryDetail dt in DetailForEdit)
            {
                dt.DetailNote = _DetailNote;
                dt.Contact = _ContactId;
                dt.UpdateDate = DateTime.Now;
            }
            db.SaveChanges();

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
