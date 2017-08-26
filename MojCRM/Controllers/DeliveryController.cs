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
        public ActionResult Index(string SortOrder, DeliverySearchHelper model)
        {
            ViewBag.InsertDateParm = String.IsNullOrEmpty(SortOrder) ? "InsertDate" : String.Empty;

            var ReferenceDate = new DateTime(2017, (DateTime.Today.AddMonths(-2)).Month, 1);
            var ResultsNew = db.DeliveryTicketModels.Where(t => t.InsertDate >= ReferenceDate);
            var OpenTickets = db.DeliveryTicketModels.Where(t => t.DocumentStatus == 30);
            var TicketsCreatedToday = db.DeliveryTicketModels.Where(t => t.InsertDate > DateTime.Today.Date);
            var TicketsCreatedTodayFirstTime = db.DeliveryTicketModels.Where(t => t.InsertDate > DateTime.Today.Date && t.FirstInvoice == true);

            ViewBag.OpenTickets = OpenTickets.Count();
            ViewBag.OpenTicketsAssigned = OpenTickets.Where(t => t.IsAssigned == true).Count();
            ViewBag.TicketsCreatedToday = TicketsCreatedToday.Count();
            ViewBag.TicketsCreatedTodayAssigned = TicketsCreatedToday.Where(t => t.IsAssigned == true).Count();
            ViewBag.TicketsCreatedTodayFirstTime = TicketsCreatedTodayFirstTime.Count();
            ViewBag.TicketsCreatedTodayFirstTimeAssigned = TicketsCreatedTodayFirstTime.Where(t => t.IsAssigned == true).Count();
            var _date = new DateTime(2017, 7, 1);
            ViewBag.TotalOpenedTickets = db.DeliveryTicketModels.Where(t => t.IsAssigned == false && t.InsertDate >= _date && t.DocumentStatus == 30).Count();

            int DocumentStatusInt;
            if (!String.IsNullOrEmpty(model.DocumentStatus))
            {
                DocumentStatusInt = Int32.Parse(model.DocumentStatus);
            }
            else
            {
                DocumentStatusInt = 30;
            };

            int DocumentTypeInt;
            if (!String.IsNullOrEmpty(model.DocumentType))
            {
                DocumentTypeInt = Int32.Parse(model.DocumentType);
            }
            else
            {
                DocumentTypeInt = 1;
            };

            if (!String.IsNullOrEmpty(model.Sender))
            {
                ResultsNew = ResultsNew.Where(t => t.Sender.SubjectName.Contains(model.Sender) || t.Sender.VAT.Contains(model.Sender));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.Receiver))
            {
                ResultsNew = ResultsNew.Where(t => t.Receiver.SubjectName.Contains(model.Receiver) || t.Receiver.VAT.Contains(model.Receiver));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.InvoiceNumber))
            {
                ResultsNew = ResultsNew.Where(t => t.InvoiceNumber.Contains(model.InvoiceNumber));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.SentDate))
            {
                var sentDate = Convert.ToDateTime(model.SentDate);
                var sentDatePlus = sentDate.AddDays(1);
                ResultsNew = ResultsNew.Where(t => (t.SentDate > sentDate) && (t.SentDate < sentDatePlus));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.TicketDate))
            {
                var insertDate = Convert.ToDateTime(model.TicketDate);
                var insertDatePlus = insertDate.AddDays(1);
                ResultsNew = ResultsNew.Where(t => (t.InsertDate > insertDate) && (t.InsertDate < insertDatePlus));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.BuyerEmail))
            {
                ResultsNew = ResultsNew.Where(t => t.BuyerEmail.Contains(model.BuyerEmail));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.DocumentStatus))
            {
                ResultsNew = ResultsNew.Where(t => t.DocumentStatus == DocumentStatusInt);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.DocumentType))
            {
                ResultsNew = ResultsNew.Where(t => t.MerDocumentTypeId == DocumentTypeInt);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.TicketType))
            {
                ResultsNew = ResultsNew.Where(t => t.FirstInvoice == true);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(model.Assigned))
            {
                if (model.Assigned == "1")
                {
                    ResultsNew = ResultsNew.Where(t => t.IsAssigned == true && t.AssignedTo == User.Identity.Name);
                    ViewBag.SearchResults = ResultsNew.Count();
                    ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
                }
                if (model.Assigned == "2")
                {
                    ResultsNew = ResultsNew.Where(t => t.IsAssigned == false);
                    ViewBag.SearchResults = ResultsNew.Count();
                    ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
                }
            }

            if (!String.IsNullOrEmpty(model.AssignedTo))
            {
                ResultsNew = ResultsNew.Where(t => t.IsAssigned == true && t.AssignedTo == model.AssignedTo);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            ViewBag.SearchResults = ResultsNew.Count();
            ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();

            switch (SortOrder)
            {
                case "InsertDate":
                    ResultsNew = ResultsNew.OrderBy(d => d.InsertDate);
                    break;
                default:
                    ResultsNew = ResultsNew.OrderByDescending(d => d.InsertDate);
                    break;
            }

            return View(ResultsNew);
        }

        // GET: Delivery/CreateTicketsFirstTime
        // Kreiranje kartica za prvo preuzimanje
        //[Authorize(Roles = "Superadmin")]
        public JsonResult CreateTicketsFirstTime(Guid? user)
        {
            var Credentials = new { MerUser = "", MerPass = "" };
            string Agent;
            if (String.IsNullOrEmpty(user.ToString()))
            {
                Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                Agent = User.Identity.Name;
            }
            else
            {
                Credentials = (from u in db.Users
                               where u.Id == user.ToString()
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                Agent = user.ToString();
            }

            int CreatedTickets = 0;
            MerApiGetNondeliveredDocuments RequestFirstTime = new MerApiGetNondeliveredDocuments()
            {
                Id = Credentials.MerUser,
                Pass = Credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                Type = 1
            };

            string MerRequestFirstTime = JsonConvert.SerializeObject(RequestFirstTime);

            using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var ResponseFirstTime = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getNondeliveredDocuments").ToString(), "POST", MerRequestFirstTime);
                //ResponseFirstTime = ResponseFirstTime.Replace("[", "").Replace("]", "");
                MerGetNondeliveredDocumentsResponse[] ResultsFirstTime = JsonConvert.DeserializeObject<MerGetNondeliveredDocumentsResponse[]>(ResponseFirstTime);
                foreach (var Result in ResultsFirstTime)
                {
                    db.DeliveryTicketModels.Add(new Delivery
                    {
                        SenderId = Result.PosiljateljId,
                        ReceiverId = Result.PrimateljId,
                        InvoiceNumber = Result.InterniBroj,
                        MerElectronicId = Result.Id,
                        SentDate = Result.DatumOtpreme,
                        MerDocumentTypeId = Result.DokumentTypeId,
                        DocumentStatus = 30,
                        InsertDate = DateTime.Now,
                        BuyerEmail = Result.EmailPrimatelja,
                        FirstInvoice = true,
                    });
                }
                db.SaveChanges();
                CreatedTickets = ResultsFirstTime.Count();
            }

            LogActivity("Kreirano kartica za prvi put: " + CreatedTickets, Agent, 0, ActivityLog.ActivityTypeEnum.SYSTEM);

            return Json(new { Status = "OK", CreatedTickets = CreatedTickets }, JsonRequestBehavior.AllowGet);
        }

        // GET: Delivery/CreateTickets
        // Kreiranje kartica za redovito preuzimanje
        //[Authorize(Roles = "Superadmin")]
        public JsonResult CreateTickets(Guid? user)
        {
            var Credentials = new { MerUser = "", MerPass = ""};
            string Agent;
            if (String.IsNullOrEmpty(user.ToString()))
            {
                Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                Agent = User.Identity.Name;
            }
            else
            {
                Credentials = (from u in db.Users
                               where u.Id == user.ToString()
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
                Agent = user.ToString();
            }

            int CreatedTickets = 0;
            MerApiGetNondeliveredDocuments RequestRegularDelivery = new MerApiGetNondeliveredDocuments()
            {
                Id = Credentials.MerUser,
                Pass = Credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                Type = 2
            };

            string MerRequestFirstTime = JsonConvert.SerializeObject(RequestRegularDelivery);

            using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var ResponseRegularDelivery = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getNondeliveredDocuments").ToString(), "POST", MerRequestFirstTime);
                //ResponseRegularDelivery = ResponseRegularDelivery.Replace("[", "").Replace("]", "");
                MerGetNondeliveredDocumentsResponse[] ResultsRegularDelivery = JsonConvert.DeserializeObject<MerGetNondeliveredDocumentsResponse[]>(ResponseRegularDelivery);
                foreach (var Result in ResultsRegularDelivery)
                {
                    db.DeliveryTicketModels.Add(new Delivery
                    {
                        SenderId = Result.PosiljateljId,
                        ReceiverId = Result.PrimateljId,
                        InvoiceNumber = Result.InterniBroj,
                        MerElectronicId = Result.Id,
                        SentDate = Result.DatumOtpreme,
                        MerDocumentTypeId = Result.DokumentTypeId,
                        DocumentStatus = 30,
                        InsertDate = DateTime.Now,
                        BuyerEmail = Result.EmailPrimatelja,
                        FirstInvoice = false,
                    });
                }
                db.SaveChanges();
                CreatedTickets = ResultsRegularDelivery.Count();
            }

            LogActivity("Kreirano kartica: " + CreatedTickets, Agent, 0, ActivityLog.ActivityTypeEnum.SYSTEM);

            return Json(new { Status = "OK", CreatedTickets = CreatedTickets }, JsonRequestBehavior.AllowGet);
        }

        // GET: Delivery/CreateTicketsOld
        [Authorize(Roles = "Superadmin")]
        [Obsolete]
        public ActionResult CreateTicketsOld(string Name)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();

            var Organizations = (from o in db.Organizations
                                select o).AsEnumerable();

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

                                bool ExistingOrganization = Organizations.Any(o => o.MerId.ToString() == Result.BuyerID.ToString());

                                if (ExistingOrganization)
                                {
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
                                else
                                {
                                    MerApiGetSubjekt Request = new MerApiGetSubjekt()
                                    {
                                        Id = MerUser.ToString(),
                                        Pass = MerPass.ToString(),
                                        Oib = "99999999927",
                                        PJ = "",
                                        SoftwareId = "MojCRM-001",
                                        SubjektPJ = Result.BuyerID
                                    };

                                    string MerRequest = JsonConvert.SerializeObject(Request);

                                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                                    {
                                        Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                        Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                                        var Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                                        Response = Response.Replace("[", "").Replace("]", "");
                                        MerGetSubjektDataResponse ResultOrg = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(Response);
                                        db.Organizations.Add(new Organizations
                                        {
                                            MerId = ResultOrg.Id,
                                            SubjectName = ResultOrg.Naziv,
                                            SubjectBusinessUnit = ResultOrg.PoslovnaJedinica,
                                            VAT = ResultOrg.Oib
                                        });
                                        db.SaveChanges();

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
                                }
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

        // GET: Delivery/UpdateStatusIndex/12345
        public ActionResult UpdateStatusIndex(int Id)
        {
            var TicketForUpdate = db.DeliveryTicketModels.Find(Id);
            var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + TicketForUpdate.MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/UpdateStatus/12345
        public ActionResult UpdateStatusDetails(int TicketId, int ReceiverId)
        {
            var TicketForUpdate = db.DeliveryTicketModels.Find(TicketId);
            var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + TicketForUpdate.MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId, Name = User.Identity.Name });
        }

        public void UpdateStatus(int Id)
        {
            var TicketForUpdate = db.DeliveryTicketModels.Find(Id);
            var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + TicketForUpdate.MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            db.SaveChanges();
        }

        // GET: Delivery/UpdateAllStatuses
        public ActionResult UpdateAllStatusesSent()
        {
            var OpenTickets = (from t in db.DeliveryTicketModels
                               where t.DocumentStatus == 30
                               select t).AsEnumerable();

            foreach (var Ticket in OpenTickets)
            {
                //var TicketForUpdate = db.DeliveryTicketModels.Find(Ticket.Id);
                var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + Ticket.MerElectronicId + "&ver=13ca6cad-60a4-4894-ba38-1a6f86b25a3c";
                MerDeliveryJsonResponse Result = ParseJson(MerString);

                Ticket.DocumentStatus = Result.Status;
                Ticket.BuyerEmail = Result.EmailPrimatelja;
                Ticket.UpdateDate = DateTime.Now;
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/UpdateAllStatuses
        public ActionResult UpdateAllStatusesOther()
        {
            var ReferenceDate = new DateTime(2017, (DateTime.Today.AddMonths(-2)).Month, 1);
            var OpenTickets = db.DeliveryTicketModels.Where(t => t.DocumentStatus != 30 && t.DocumentStatus != 40 && t.DocumentStatus != 55);

            foreach (var Ticket in OpenTickets)
            {
                var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + Ticket.MerElectronicId + "&ver=13ca6cad-60a4-4894-ba38-1a6f86b25a3c";
                MerDeliveryJsonResponse Result = ParseJson(MerString);

                Ticket.DocumentStatus = Result.Status;
                Ticket.BuyerEmail = Result.EmailPrimatelja;
                Ticket.UpdateDate = DateTime.Now;
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/Resend/12345
        [Authorize]
        public ActionResult Resend(int TicketId, int MerElectronicId, int ReceiverId, bool IsTicket = true)
        {
            var InvoiceNumber = "";
            var Credentials = new { MerUser = "", MerPass = "" };
            Credentials = (from u in db.Users
                           where u.UserName == User.Identity.Name
                           select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            if (IsTicket == true)
            {
                InvoiceNumber = (from t in db.DeliveryTicketModels
                                     where t.Id == TicketId
                                     select t.InvoiceNumber).First();
            }
            else
            {
                InvoiceNumber = null;
            }
            var Receiver = (from o in db.Organizations
                            where o.MerId == ReceiverId //&& o.SubjectBusinessUnit == ""
                            select o.SubjectName).First();

            MerApiResend Request = new MerApiResend()
            {
                Id = Credentials.MerUser,
                Pass = Credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                DocumentId = MerElectronicId
            };

            string MerRequest = JsonConvert.SerializeObject(Request);

            using (var Mer = new WebClient())
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/changeEmail").ToString(), "POST", MerRequest);
            }

            if (InvoiceNumber == null)
            {
                LogActivity(User.Identity.Name + " je ponovno poslao obavijest za primatelja " + Receiver, User.Identity.Name, TicketId, ActivityLog.ActivityTypeEnum.RESEND);
            }
            else
            {
                LogActivity(User.Identity.Name + " je ponovno poslao obavijest za dokument " + InvoiceNumber, User.Identity.Name, TicketId, ActivityLog.ActivityTypeEnum.RESEND);
            }

            return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId });
        }

        // GET Delivery/ChangeEmail
        [Authorize]
        public ActionResult ChangeEmail(ChangeEmailHelper model)
        {
            var Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            MerApiChangeEmail Request = new MerApiChangeEmail()
            {
                Id = Credentials.MerUser,
                Pass = Credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                DocumentId = model.MerElectronicId,
                Email = model.NewEmail
            };

            string MerRequest = JsonConvert.SerializeObject(Request);

            using (var Mer = new WebClient())
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/changeEmail").ToString(), "POST", MerRequest);
            }

            LogActivity(User.Identity.Name + " je izmijenio e-mail adresu za dostavu eDokumenta iz " + model.OldEmail + " u " + model.NewEmail + " i ponovno poslao obavijest za dokument broj: " + model.InvoiceNumber, 
                User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.MAILCHANGE);
            UpdateStatus(model.TicketId);

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
        }

        // GET: Delivery/ChangeEmailNoTicket
        [Authorize]
        public ActionResult ChangeEmailNoTicket(ChangeEmailHelper model)
        {
            var Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            MerApiChangeEmail Request = new MerApiChangeEmail()
            {
                Id = Credentials.MerUser,
                Pass = Credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                DocumentId = model.MerElectronicId,
                Email = model.NewEmail
            };

            string MerRequest = JsonConvert.SerializeObject(Request);

            using (var Mer = new WebClient())
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/changeEmail").ToString(), "POST", MerRequest);
            }

            LogActivity(User.Identity.Name + " je izmijenio e-mail adresu za dostavu eDokumenta iz " + model.OldEmail + " u " + model.NewEmail + " i ponovno poslao obavijest za dokument broj: " + model.InvoiceNumber,
                User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.MAILCHANGE);

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
        }

        // POST Delivery/Remove/1125768
        [HttpPost]
        public JsonResult Remove(int MerElectronicId, int TicketId)
        {
            var TicketForRemoval = db.DeliveryTicketModels.Find(TicketId);
            TicketForRemoval.DocumentStatus = 55;
            TicketForRemoval.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // POST Delivery/RemoveAll
        [HttpPost]
        public JsonResult RemoveAll(int[] TicketIds)
        {
            foreach (var Id in TicketIds)
            {
                var TicketForRemoval = db.DeliveryTicketModels.Find(Id);
                TicketForRemoval.DocumentStatus = 55;
                TicketForRemoval.UpdateDate = DateTime.Now;
            }
            db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // POST Delivery/AssignAll
        [HttpPost]
        public JsonResult AssignAll(int[] TicketIds)
        {
            foreach (var Id in TicketIds)
            {
                var TicketForAssign = db.DeliveryTicketModels.Find(Id);
                TicketForAssign.IsAssigned = true;
                TicketForAssign.AssignedTo = User.Identity.Name;

                db.ActivityLogs.Add(new ActivityLog()
                {
                    Description = "Agent " + User.Identity.Name + " si je dodijelio karticu za primatelja: " + TicketForAssign.Receiver.SubjectName + ", za eDokument: " + TicketForAssign.InvoiceNumber,
                    User = User.Identity.Name,
                    ReferenceId = Id,
                    ActivityType = ActivityLog.ActivityTypeEnum.TICKETASSIGN,
                    Department = ActivityLog.DepartmentEnum.Delivery,
                    Module = ActivityLog.ModuleEnum.Delivery,
                    InsertDate = DateTime.Now
                });
            }
            db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // GET: Delivery/Details/5
        [Authorize]
        public ActionResult Details(int id, int? receiverId)
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

            //UpdateStatus(ElectronicId);

            DateTime ReferenceDate = DateTime.Now.AddMonths(-2);

            var _RelatedInvoicesList = (from t in db.DeliveryTicketModels
                                        where t.Id != id && t.ReceiverId == deliveryTicketModel.ReceiverId && t.SentDate > ReferenceDate && t.DocumentStatus == 30
                                        select t).AsEnumerable();

            var _RelatedDeliveryContacts = (from t in db.Contacts
                                            where t.Organization.MerId == receiverId && t.ContactType == Contact.ContactTypeEnum.DELIVERY
                                            select t).AsEnumerable();

            var _RelatedDeliveryDetails = (from t in db.DeliveryDetails
                                           where t.Receiver.MerId == receiverId
                                           select t).AsEnumerable().OrderByDescending(t => t.Id);

            var _ImportantComment = (from dd in db.MerDeliveryDetails
                                     where dd.MerId == receiverId
                                     select dd.ImportantComments).First();

            var _RelatedActivities = (from a in db.ActivityLogs
                                      where a.ReferenceId == id
                                      select a).AsEnumerable().OrderByDescending(a => a.Id);

            #region Postmark API
            MessagesOutboundOpenResponse OpeningHistoryResponse;
            BouncesResponse Bounces;
            string PostmarkLinkOpeningHistory = @"https://api.postmarkapp.com/messages/outbound/opens?count=20&offset=0&recipient=" + deliveryTicketModel.BuyerEmail;
            string PostmarkBounces = @"https://api.postmarkapp.com/bounces?count=20&offset=0&emailFilter=" + deliveryTicketModel.BuyerEmail;
            using (var Postmark = new WebClient())
            {
                Postmark.Headers.Add(HttpRequestHeader.Accept, "application/json");
                Postmark.Headers.Add("X-Postmark-Server-Token", "8ab33a3d-a800-405f-afad-9c75c2f08c0b");
                var OpeningHistoryRequest = Postmark.DownloadString(PostmarkLinkOpeningHistory);
                OpeningHistoryResponse = JsonConvert.DeserializeObject<MessagesOutboundOpenResponse>(OpeningHistoryRequest);
            }

            using (var Postmark = new WebClient())
            {
                Postmark.Headers.Add(HttpRequestHeader.Accept, "application/json");
                Postmark.Headers.Add("X-Postmark-Server-Token", "8ab33a3d-a800-405f-afad-9c75c2f08c0b");
                var BouncesRequest = Postmark.DownloadString(PostmarkBounces);
                Bounces = JsonConvert.DeserializeObject<BouncesResponse>(BouncesRequest);
            }
            #endregion


            var Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();

            MerApiGetSentDocuments RequestGetSentDocuments = new MerApiGetSentDocuments()
            {
                Id = Credentials.MerUser,
                Pass = Credentials.MerPass,
                Oib = "99999999927",
                PJ = "",
                SoftwareId = "MojCRM-001",
                SubjektPJ = receiverId.ToString(),
                Take = 20
            };

            string MerRequest = JsonConvert.SerializeObject(RequestGetSentDocuments);

            using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var ResponseRegularDelivery = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSentDocuments").ToString(), "POST", MerRequest);
                //ResponseRegularDelivery = ResponseRegularDelivery.Replace("[", "").Replace("]", "");
                MerGetSentDocumentsResponse[] ResultsDocumentHistory = JsonConvert.DeserializeObject<MerGetSentDocumentsResponse[]>(ResponseRegularDelivery);

                var DeliveryDetails = new DeliveryDetailsViewModel
                {
                    TicketId = id,
                    SenderName = deliveryTicketModel.Sender.SubjectName,
                    ReceiverName = deliveryTicketModel.Receiver.SubjectName,
                    InvoiceNumber = deliveryTicketModel.InvoiceNumber,
                    SentDate = deliveryTicketModel.SentDate,
                    MerDocumentTypeId = deliveryTicketModel.MerDocumentTypeId,
                    MerDocumentStatusId = deliveryTicketModel.DocumentStatus,
                    ReceiverEmail = deliveryTicketModel.BuyerEmail,
                    MerDeliveryDetailComment = deliveryTicketModel.Receiver.MerDeliveryDetail.Comments,
                    MerDeliveryDetailTelephone = deliveryTicketModel.Receiver.MerDeliveryDetail.Telephone,
                    ImportantComment = _ImportantComment,
                    MerElectronicId = deliveryTicketModel.MerElectronicId,
                    ReceiverId = deliveryTicketModel.ReceiverId,
                    ReceiverVAT = deliveryTicketModel.Receiver.VAT,
                    SenderVAT = deliveryTicketModel.Sender.VAT,
                    RelatedInvoices = _RelatedInvoicesList,
                    RelatedDeliveryContacts = _RelatedDeliveryContacts,
                    RelatedDeliveryDetails = _RelatedDeliveryDetails,
                    RelatedActivities = _RelatedActivities,
                    IsAssigned = deliveryTicketModel.IsAssigned,
                    AssignedTo = deliveryTicketModel.AssignedTo,
                    DocumentHistory = ResultsDocumentHistory.Where(i => (i.DokumentStatusId != 10) && (i.DokumentTypeId != 6 && i.DokumentTypeId != 632)).AsEnumerable(),
                    PostmarkOpenings = OpeningHistoryResponse,
                    PostmarkBounces = Bounces
                };

                return View(DeliveryDetails);
            }
        }

        // GET: Delivery/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddDetail/1125768
        [HttpPost]
        public ActionResult AddDetail(DeliveryDetailHelper model)
        {
            var contact = db.Contacts.Find(model.Contact);
            if (model.DetailTemplate == String.Empty && model.DetailNote != String.Empty)
            {
                try
                {
                    db.DeliveryDetails.Add(new DeliveryDetail
                    {
                        ReceiverId = model.ReceiverId,
                        User = User.Identity.Name,
                        DetailNote = model.DetailNote,
                        InsertDate = DateTime.Now,
                        Contact = contact.ContactFirstName + " " + contact.ContactLastName,
                        TicketId = model.TicketId
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
                        User = User.Identity.Name,
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
                        User = User.Identity.Name,
                        InsertDate = DateTime.Now
                    });
                    db.SaveChanges();
                }
            }
            else if (model.DetailNote == String.Empty && model.DetailTemplate != String.Empty)
            {
                db.DeliveryDetails.Add(new DeliveryDetail
                {
                    ReceiverId = model.ReceiverId,
                    User = User.Identity.Name,
                    DetailNote = model.DetailTemplate,
                    InsertDate = DateTime.Now,
                    Contact = contact.ContactFirstName + " " + contact.ContactLastName,
                    TicketId = model.TicketId
                });
                db.SaveChanges();
            }
            else
            {
                db.DeliveryDetails.Add(new DeliveryDetail
                {
                    ReceiverId = model.ReceiverId,
                    User = User.Identity.Name,
                    DetailNote = model.DetailTemplate + " - " + model.DetailNote,
                    InsertDate = DateTime.Now,
                    Contact = contact.ContactFirstName + " " + contact.ContactLastName,
                    TicketId = model.TicketId
                });
                db.SaveChanges();
            }

            switch (model.Identifier)
            {
                case 1:
                    LogActivity(User.Identity.Name + " je obavio uspješan poziv za dostavu eDokumenata broj: " + model.InvoiceNumber,
                        User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.SUCCALL);
                    break;
                case 2:
                    LogActivity(User.Identity.Name + " je obavio kraći informativni poziv vezano za dostavu eDokumenata broj: " + model.InvoiceNumber,
                        User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.SUCCALSHORT);
                    break;
                case 3:
                    LogActivity(User.Identity.Name + " je pokušao obaviti telefonski poziv vezano za dostavu eDokumenata broj: " + model.InvoiceNumber,
                        User.Identity.Name, model.TicketId, ActivityLog.ActivityTypeEnum.UNSUCCAL);
                    break;
            }

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
        }

        // POST: Delivery/EditDetail/12345
        [HttpPost]
        [Authorize]
        public ActionResult EditDetail(DeliveryDetailHelper model)
        {
            var DetailForEdit = db.DeliveryDetails.Find(model.DetailNoteId);
            var contact = db.Contacts.Find(model.Contact);

            DetailForEdit.DetailNote = model.DetailNote;
            DetailForEdit.Contact = contact.ContactFirstName + " " + contact.ContactLastName;
            DetailForEdit.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = model.TicketId, receiverId = model.ReceiverId });
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

        // POST: Delivery/DeleteDetail/5
        [HttpPost, ActionName("DeleteDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDetail(int DetailId, string TicketId, string ReceiverId)
        {
            int TicketIdInt = Int32.Parse(TicketId);
            int ReceiverIdInt = Int32.Parse(ReceiverId);
            DeliveryDetail deliveryDetail = db.DeliveryDetails.Find(DetailId);
            db.DeliveryDetails.Remove(deliveryDetail);
            db.SaveChanges();
            return RedirectToAction("Details", "Delivery", new { id = TicketIdInt, receiverId = ReceiverIdInt });
        }

        // POST: Delivery/Contacts
        public ActionResult Contacts(string Organization, string ContactName, string Number, string Email)
        {
            var Results = from c in db.Contacts
                          where c.ContactType == Contact.ContactTypeEnum.DELIVERY
                          select c;
            if (!String.IsNullOrEmpty(Organization))
            {
                Results = Results.Where(c => c.Organization.SubjectName.Contains(Organization) || c.Organization.VAT.Contains(Organization));
            }
            if (!String.IsNullOrEmpty(ContactName))
            {
                Results = Results.Where(c => c.ContactFirstName.Contains(ContactName) || c.ContactLastName.Contains(ContactName));
            }
            if (!String.IsNullOrEmpty(Number))
            {
                Results = Results.Where(c => c.TelephoneNumber.Contains(Number) || c.MobilePhoneNumber.Contains(Number));
            }
            if (!String.IsNullOrEmpty(Email))
            {
                Results = Results.Where(c => c.Email.Contains(Email));
            }

            return View(Results);
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

        public void LogEmail(int _TicketId, string _Email)
        {
            var ticket = db.DeliveryTicketModels.Find(_TicketId);

            LogActivity(User.Identity.Name + " je poslao e-mail na adresu: " + _Email + " na temu dostave eDokumenata broj: " + ticket.InvoiceNumber,
                User.Identity.Name, _TicketId, ActivityLog.ActivityTypeEnum.EMAIL);
        }

        public JsonResult Assign(int Id)
        {
            var TicketForAssignement = db.DeliveryTicketModels.Find(Id);
            TicketForAssignement.IsAssigned = true;
            TicketForAssignement.AssignedTo = User.Identity.Name;
            LogActivity("Agent " + User.Identity.Name + " si je dodijelio karticu za primatelja: " + TicketForAssignement.Receiver.SubjectName + ", za eDokument: " + TicketForAssignement.InvoiceNumber,
                User.Identity.Name, Id, ActivityLog.ActivityTypeEnum.TICKETASSIGN);

            return Json(new { Status = "OK" });
        }

        public ActionResult Reassing(bool Unassign, string ReassignedTo, string TicketId)
        {
            int TicketIdInt = Int32.Parse(TicketId);
            var TicketForReassingement = (from t in db.DeliveryTicketModels
                                          where t.Id == TicketIdInt
                                          select t).First();
            if (Unassign == true)
            {
                TicketForReassingement.IsAssigned = false;
                TicketForReassingement.AssignedTo = String.Empty;
                db.SaveChanges();
            }
            else
            {
                TicketForReassingement.AssignedTo = ReassignedTo;
                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        //POST: Delivery/AssignManagement
        [HttpPost, ActionName("AssignManagement")]
        public JsonResult AssignManagement(AssigningTickets[] forAssign)
        {
            foreach (var Assign in forAssign)
            {
                if (Assign.TicketDate == null)
                {
                    var _sentDate = Convert.ToDateTime(Assign.SentDate);
                    var TicketsForAssing = (from t in db.DeliveryTicketModels
                                            where (t.InsertDate >= DateTime.Today) && (t.SentDate >= _sentDate)
                                            select t).AsEnumerable();
                    foreach (var Ticket in TicketsForAssing)
                    {
                        Ticket.IsAssigned = true;
                        Ticket.AssignedTo = Assign.Agent;
                    }
                    db.SaveChanges();
                }
                else
                {
                    var _ticketDate = Convert.ToDateTime(Assign.TicketDate);
                    var _sentDate = Convert.ToDateTime(Assign.SentDate);
                    var TicketsForAssing = (from t in db.DeliveryTicketModels
                                            where (t.InsertDate == _ticketDate) && (t.SentDate == _sentDate)
                                            select t).AsEnumerable();
                    foreach (var Ticket in TicketsForAssing)
                    {
                        Ticket.IsAssigned = true;
                        Ticket.AssignedTo = Assign.Agent;
                    }
                    db.SaveChanges();
                }
            }
            return Json(new { Status = "OK" });
        }

        // POST: Delivery/PostmarkActivateBounce
        [HttpPost]
        public JsonResult PostmarkActivateBounce(int BounceId, int TicketId)
        {
            ActivateBounceResponse ActivateResponse;
            string ActivateLink = String.Format(@"https://api.postmarkapp.com/bounces/" + BounceId + "/activate");

            using (var Postmark = new WebClient())
            {
                Postmark.Headers.Add(HttpRequestHeader.Accept, "application/json");
                Postmark.Headers.Add("X-Postmark-Server-Token", "8ab33a3d-a800-405f-afad-9c75c2f08c0b");
                var Response = Postmark.UploadString(ActivateLink, "PUT", ActivateLink);
                ActivateResponse = JsonConvert.DeserializeObject<ActivateBounceResponse>(Response);

                LogActivity(User.Identity.Name + " je reaktivirao e-mail adresu " + ActivateResponse.Bounce.Email + " u Postmarku",
                    User.Identity.Name, TicketId, ActivityLog.ActivityTypeEnum.POSTMARKACTIVATEBOUNCE);
            }

            return Json(new { Status = "OK" });
        }

        public void LogActivity(string ActivityDescription, string User, int ActivityReferenceId, ActivityLog.ActivityTypeEnum ActivityType)
        {
            db.ActivityLogs.Add(new ActivityLog
            {
                Description = ActivityDescription,
                User = User,
                ReferenceId = ActivityReferenceId,
                ActivityType = ActivityType,
                Department = ActivityLog.DepartmentEnum.Delivery,
                Module = ActivityLog.ModuleEnum.Delivery,
                InsertDate = DateTime.Now
            });
            db.SaveChanges();
        }
    }
}
