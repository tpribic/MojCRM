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
                                               string SentDate, string TicketDate, string BuyerEmail, string DocumentStatus, 
                                               string DocumentType, string TicketType, string Assigned, string AssignedTo)
        {
            ViewBag.InsertDateParm = String.IsNullOrEmpty(SortOrder) ? "InsertDate" : String.Empty;

            var ResultsNew = from t in db.DeliveryTicketModels
                          where t.DocumentStatus == 30
                          select t;
            //var ResultsNew = db.DeliveryTicketModels.AsQueryable();
            var TicketsCreatedToday = from t in db.DeliveryTicketModels
                                      where t.InsertDate > DateTime.Today.Date
                                      select t;
            var TicketsCreatedTodayFirstTime = from t in db.DeliveryTicketModels
                                               where t.InsertDate > DateTime.Today.Date && t.FirstInvoice == true
                                               select t;

            ViewBag.OpenTickets = ResultsNew.Count();
            ViewBag.OpenTicketsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            ViewBag.TicketsCreatedToday = TicketsCreatedToday.Count();
            ViewBag.TicketsCreatedTodayAssigned = TicketsCreatedToday.Where(t => t.IsAssigned == true).Count();
            ViewBag.TicketsCreatedTodayFirstTime = TicketsCreatedTodayFirstTime.Count();
            ViewBag.TicketsCreatedTodayFirstTimeAssigned = TicketsCreatedTodayFirstTime.Where(t => t.IsAssigned == true).Count();

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
                ResultsNew = ResultsNew.Where(t => t.Sender.SubjectName.Contains(Sender) || t.Sender.VAT.Contains(Sender));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(Receiver))
            {
                ResultsNew = ResultsNew.Where(t => t.Receiver.SubjectName.Contains(Receiver) || t.Receiver.VAT.Contains(Receiver));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(InvoiceNumber))
            {
                ResultsNew = ResultsNew.Where(t => t.InvoiceNumber.Contains(InvoiceNumber));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(SentDate))
            {
                var sentDate = Convert.ToDateTime(SentDate);
                var sentDatePlus = sentDate.AddDays(1);
                ResultsNew = ResultsNew.Where(t => (t.SentDate > sentDate) && (t.SentDate < sentDatePlus));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(TicketDate))
            {
                var insertDate = Convert.ToDateTime(TicketDate);
                var insertDatePlus = insertDate.AddDays(1);
                ResultsNew = ResultsNew.Where(t => (t.InsertDate > insertDate) && (t.InsertDate < insertDatePlus));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(BuyerEmail))
            {
                ResultsNew = ResultsNew.Where(t => t.BuyerEmail.Contains(BuyerEmail));
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(DocumentStatus))
            {
                ResultsNew = ResultsNew.Where(t => t.DocumentStatus == DocumentStatusInt);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(DocumentType))
            {
                ResultsNew = ResultsNew.Where(t => t.MerDocumentTypeId == DocumentTypeInt);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(TicketType))
            {
                ResultsNew = ResultsNew.Where(t => t.FirstInvoice == true);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(Assigned))
            {
                ResultsNew = ResultsNew.Where(t => t.IsAssigned == true && t.AssignedTo == User.Identity.Name);
                ViewBag.SearchResults = ResultsNew.Count();
                ViewBag.SearchResultsAssigned = ResultsNew.Where(t => t.IsAssigned == true).Count();
            }

            if (!String.IsNullOrEmpty(AssignedTo))
            {
                ResultsNew = ResultsNew.Where(t => t.IsAssigned == true && t.AssignedTo == AssignedTo);
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

            return View(await ResultsNew.ToListAsync());
        }

        // GET: Delivery/CreateTicketsFirstTime
        // Kreiranje kartica za prvo preuzimanje
        public void CreateTicketsFirstTime(string Name)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();
            var Organizations = (from o in db.Organizations
                                 select o).AsEnumerable();

            MerApiGetNondeliveredDocuments RequestFirstTime = new MerApiGetNondeliveredDocuments();

            RequestFirstTime.Id = MerUser;
            RequestFirstTime.Pass = MerPass;
            RequestFirstTime.Oib = "99999999927";
            RequestFirstTime.PJ = "";
            RequestFirstTime.SoftwareId = "MojCRM-001";
            RequestFirstTime.Type = 1;

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
                    bool ExistingOrganization = Organizations.Any(o => o.MerId == Result.PrimateljId);
                    if (ExistingOrganization)
                    {
                        db.DeliveryTicketModels.Add(new Delivery
                        {
                            SenderId = Result.PosiljateljId,
                            ReceiverId = Result.PrimateljId,
                            InvoiceNumber = Result.InterniBroj,
                            //MerJson = Result,
                            MerElectronicId = Result.Id,
                            SentDate = Result.DatumOtpreme,
                            MerDocumentTypeId = Result.DokumentTypeId,
                            DocumentStatus = 30,
                            InsertDate = DateTime.Now,
                            BuyerEmail = Result.EmailPrimatelja,
                            FirstInvoice = true,
                        });
                    }
                    else
                    {
                        MerApiGetSubjekt RequestGetNonexistingSubjekt = new MerApiGetSubjekt();

                        RequestGetNonexistingSubjekt.Id = MerUser.ToString();
                        RequestGetNonexistingSubjekt.Pass = MerPass.ToString();
                        RequestGetNonexistingSubjekt.Oib = "99999999927";
                        RequestGetNonexistingSubjekt.PJ = "";
                        RequestGetNonexistingSubjekt.SoftwareId = "MojCRM-001";
                        RequestGetNonexistingSubjekt.SubjektPJ = Result.PrimateljId.ToString();

                        string MerRequest = JsonConvert.SerializeObject(RequestGetNonexistingSubjekt);

                        Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                        var ResponseNonExistingSubjekt = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                        ResponseNonExistingSubjekt = ResponseNonExistingSubjekt.Replace("[", "").Replace("]", "");
                        MerGetSubjektDataResponse ResultNonExistingSubjekt = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(ResponseNonExistingSubjekt);
                        db.Organizations.Add(new Organizations
                        {
                            MerId = ResultNonExistingSubjekt.Id,
                            SubjectName = ResultNonExistingSubjekt.Naziv,
                            SubjectBusinessUnit = ResultNonExistingSubjekt.PoslovnaJedinica,
                            VAT = ResultNonExistingSubjekt.Oib
                        });
                        db.SaveChanges();

                        db.DeliveryTicketModels.Add(new Delivery
                        {
                            SenderId = Result.PosiljateljId,
                            ReceiverId = Result.PrimateljId,
                            InvoiceNumber = Result.InterniBroj,
                            //MerJson = Result,
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
                }
            }
        }

        // GET: Delivery/CreateTickets
        // Kreiranje kartica za redovito preuzimanje
        public void CreateTickets(string Name)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();

            MerApiGetNondeliveredDocuments RequestRegularDelivery = new MerApiGetNondeliveredDocuments();

            RequestRegularDelivery.Id = MerUser;
            RequestRegularDelivery.Pass = MerPass;
            RequestRegularDelivery.Oib = "99999999927";
            RequestRegularDelivery.PJ = "";
            RequestRegularDelivery.SoftwareId = "MojCRM-001";
            RequestRegularDelivery.Type = 2;

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
                        //MerJson = Result,
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
            }
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
                                    MerApiGetSubjekt Request = new MerApiGetSubjekt();

                                    Request.Id = MerUser.ToString();
                                    Request.Pass = MerPass.ToString();
                                    Request.Oib = "99999999927";
                                    Request.PJ = "";
                                    Request.SoftwareId = "MojCRM-001";
                                    Request.SubjektPJ = Result.BuyerID;

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
        public ActionResult UpdateStatusIndex(int MerElectronicId)
        {
            var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            var TicketForUpdate = (from t in db.DeliveryTicketModels
                                   where t.MerElectronicId == MerElectronicId
                                   select t).First();
            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/UpdateStatus/12345
        public ActionResult UpdateStatusDetails(int TicketId, int MerElectronicId, int ReceiverId)
        {
            var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + MerElectronicId + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            var TicketForUpdate = (from t in db.DeliveryTicketModels
                                   where t.MerElectronicId == MerElectronicId
                                   select t).First();
            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId, Name = User.Identity.Name });
        }

        public void UpdateStatus(int Id)
        {
            var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + Id + "&ver=5115e32c-6be4-4a92-8e92-afe122e99d1c";

            MerDeliveryJsonResponse Response = ParseJson(MerString);

            var TicketForUpdate = (from t in db.DeliveryTicketModels
                                  where t.MerElectronicId == Id
                                  select t).First();
            TicketForUpdate.DocumentStatus = Response.Status;
            TicketForUpdate.BuyerEmail = Response.EmailPrimatelja;
            TicketForUpdate.UpdateDate = DateTime.Now;
            db.SaveChanges();
        }

        // GET: Delivery/UpdateAllStatuses
        public ActionResult UpdateAllStatuses()
        {
            var OpenTickets = from t in db.DeliveryTicketModels
                              where t.DocumentStatus == 30
                              select t;
            var MerLinks = OpenTickets.AsEnumerable();

            foreach (var Link in MerLinks)
            {
                var MerString = "https://www.moj-eracun.hr/exchange/getstatus?id=" + Link.MerElectronicId + "&ver=13ca6cad-60a4-4894-ba38-1a6f86b25a3c";
                MerDeliveryJsonResponse Result = ParseJson(MerString);

                var TicketForUpdate = (from t in db.DeliveryTicketModels
                                       where t.MerElectronicId == Link.MerElectronicId
                                       select t).First();
                TicketForUpdate.DocumentStatus = Result.Status;
                TicketForUpdate.BuyerEmail = Result.EmailPrimatelja;
                TicketForUpdate.UpdateDate = DateTime.Now;
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Delivery/Resend/12345
        [Authorize]
        public ActionResult Resend(int TicketId, int MerElectronicId, int ReceiverId, string Name)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();
            var InvoiceNumber = (from t in db.DeliveryTicketModels
                                 where t.MerElectronicId == MerElectronicId
                                 select t.InvoiceNumber).First();
            var Receiver = (from o in db.Organizations
                            where o.MerId == ReceiverId && o.SubjectBusinessUnit == null
                            select o.SubjectName).First();

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

            if (InvoiceNumber == null)
            {
                db.ActivityLogs.Add(new ActivityLog
                {
                    Description = Name + " je ponovno poslao obavijest za primatelja " + Receiver,
                    User = Name,
                    ReferenceId = TicketId,
                    ActivityType = ActivityLog.ActivityTypeEnum.RESEND,
                    Department = ActivityLog.DepartmentEnum.Delivery,
                    InsertDate = DateTime.Now,
                });
                db.SaveChanges();
            }
            else
            {
                db.ActivityLogs.Add(new ActivityLog
                {
                    Description = Name + " je ponovno poslao obavijest za dokument " + InvoiceNumber,
                    User = Name,
                    ReferenceId = TicketId,
                    ActivityType = ActivityLog.ActivityTypeEnum.RESEND,
                    Department = ActivityLog.DepartmentEnum.Delivery,
                    InsertDate = DateTime.Now,
                });
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = TicketId, receiverId = ReceiverId, Name = User.Identity.Name });
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

            var OldEmail = (from t in db.DeliveryTicketModels
                            where t.Id == _TicketIdInt && t.MerElectronicId == _IdInt
                            select t.BuyerEmail).First();
            var InvoiceNumber = (from t in db.DeliveryTicketModels
                                 where t.Id == _TicketIdInt && t.MerElectronicId == _IdInt
                                 select t.InvoiceNumber).First();

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

            db.ActivityLogs.Add(new ActivityLog
            {
                Description = _Agent + " je izmijenio e-mail adresu za dostavu eDokumenta iz " + OldEmail + " u " + _Email + " i ponovno poslao obavijest za dokument broj: " + InvoiceNumber,
                User = _Agent,
                ReferenceId = _TicketIdInt,
                ActivityType = ActivityLog.ActivityTypeEnum.MAILCHANGE,
                Department = ActivityLog.DepartmentEnum.Delivery,
                InsertDate = DateTime.Now,
            });
            db.SaveChanges();

            UpdateStatus(_IdInt);

            return RedirectToAction("Details", new { id = _TicketIdInt, receiverId = _ReceiverInt, Name = User.Identity.Name });
        }

        // POST Delivery/Remove/1125768
        [HttpPost]
        public JsonResult Remove(int MerElectronicId, int TicketId)
        {
            var TicketForRemoval = (from t in db.DeliveryTicketModels
                                    where t.MerElectronicId == MerElectronicId && t.Id == TicketId
                                    select t).First();

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
                var TicketForRemoval = (from t in db.DeliveryTicketModels
                                        where t.Id == Id
                                        select t).First();

                TicketForRemoval.DocumentStatus = 55;
                TicketForRemoval.UpdateDate = DateTime.Now;
            }
            db.SaveChanges();

            return Json(new { Status = "OK" });
        }

        // GET: Delivery/Details/5
        [Authorize]
        public ActionResult Details(int id, int? receiverId, string Name)
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

            var ElectronicId = (from t in db.DeliveryTicketModels
                                where t.Id == id
                                select t.MerElectronicId).First();
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

            var _RelatedActivities = (from a in db.ActivityLogs
                                      where a.ReferenceId == id
                                      select a).AsEnumerable().OrderByDescending(a => a.Id);

            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();

            MerApiGetSentDocuments RequestGetSentDocuments = new MerApiGetSentDocuments();

            RequestGetSentDocuments.Id = MerUser;
            RequestGetSentDocuments.Pass = MerPass;
            RequestGetSentDocuments.Oib = "99999999927";
            RequestGetSentDocuments.PJ = "";
            RequestGetSentDocuments.SoftwareId = "MojCRM-001";
            RequestGetSentDocuments.SubjektPJ = receiverId.ToString();
            RequestGetSentDocuments.Take = 20;

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
                    DocumentHistory = ResultsDocumentHistory.Where(i => i.DokumentStatusId != 10).AsEnumerable()
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
        public ActionResult AddDetail(int _ReceiverId, string _Agent, string _ContactId, string _DetailTemplate, string _DetailNote,
                                      int _TicketId, int Identifier)
        {
            var InvoiceNumber = (from t in db.DeliveryTicketModels
                                 where t.Id == _TicketId
                                 select t.InvoiceNumber).First();

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

            switch (Identifier)
            {
                case 1:
                    db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = _Agent + " je obavio uspješan poziv za dostavu eDokumenata broj: " + InvoiceNumber,
                        User = _Agent,
                        ReferenceId = _TicketId,
                        ActivityType = ActivityLog.ActivityTypeEnum.SUCCALL,
                        Department = ActivityLog.DepartmentEnum.Delivery,
                        InsertDate = DateTime.Now,
                    });
                    db.SaveChanges();
                    break;
                case 2:
                    db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = _Agent + " je obavio kraći informativni poziv vezano za dostavu eDokumenata broj: " + InvoiceNumber,
                        User = _Agent,
                        ReferenceId = _TicketId,
                        ActivityType = ActivityLog.ActivityTypeEnum.SUCCALSHORT,
                        Department = ActivityLog.DepartmentEnum.Delivery,
                        InsertDate = DateTime.Now,
                    });
                    db.SaveChanges();
                    break;
                case 3:
                    db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = _Agent + " je pokušao obaviti telefonski poziv vezano za dostavu eDokumenata broj: " + InvoiceNumber,
                        User = _Agent,
                        ReferenceId = _TicketId,
                        ActivityType = ActivityLog.ActivityTypeEnum.UNSUCCAL,
                        Department = ActivityLog.DepartmentEnum.Delivery,
                        InsertDate = DateTime.Now,
                    });
                    db.SaveChanges();
                    break;
            }

            return RedirectToAction("Details", new { id = _TicketId, receiverId = _ReceiverId, Name = User.Identity.Name });
        }

        // POST: Delivery/EditDetail/12345
        [HttpPost]
        [Authorize]
        public ActionResult EditDetail(int _ReceiverId, string _Agent, string _ContactId, string _DetailNoteId, string _DetailNote, string _TicketId)
        {
            int _TicketIdInt = Int32.Parse(_TicketId);
            int _DetailNoteIdInt = Int32.Parse(_DetailNoteId);
            var DetailForEdit = (from t in db.DeliveryDetails
                                where t.Id == _DetailNoteIdInt
                                select t).First();

            DetailForEdit.DetailNote = _DetailNote;
            DetailForEdit.Contact = _ContactId;
            DetailForEdit.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = _TicketId, receiverId = _ReceiverId, Name = User.Identity.Name });
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
            return RedirectToAction("Details", "Delivery", new { id = TicketIdInt, receiverId = ReceiverIdInt, Name = User.Identity.Name });
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

            return View(Results.ToList());
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

        public void LogEmail(string _Agent, int _TicketId, string _Email)
        {
            var InvoiceNumber = (from t in db.DeliveryTicketModels
                                 where t.Id == _TicketId
                                 select t.InvoiceNumber).First();

            db.ActivityLogs.Add(new ActivityLog
            {
                Description = _Agent + " je poslao e-mail na adresu: " + _Email + " na temu dostave eDokumenata broj: " + InvoiceNumber,
                User = _Agent,
                ReferenceId = _TicketId,
                ActivityType = ActivityLog.ActivityTypeEnum.DELMAIL,
                Department = ActivityLog.DepartmentEnum.Delivery,
                InsertDate = DateTime.Now,
            });
            db.SaveChanges();
        }

        public JsonResult Assign(int Id, string Agent)
        {
            var TicketForAssignement = (from t in db.DeliveryTicketModels
                                        where t.Id == Id
                                        select t).First();
            TicketForAssignement.IsAssigned = true;
            TicketForAssignement.AssignedTo = Agent;
            db.SaveChanges();

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
        [HttpPost]
        public void AssignManagement (List<AssigningTickets> forAssign)
        {
            foreach (var Assign in forAssign)
            {
                if (Assign.TicketDate == null)
                {
                    var TicketsForAssing = (from t in db.DeliveryTicketModels
                                            where (t.InsertDate == DateTime.Today) && (t.SentDate == Assign.SentDate)
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
                    var TicketsForAssing = (from t in db.DeliveryTicketModels
                                            where (t.InsertDate == Assign.TicketDate) && (t.SentDate == Assign.SentDate)
                                            select t).AsEnumerable();
                    foreach (var Ticket in TicketsForAssing)
                    {
                        Ticket.IsAssigned = true;
                        Ticket.AssignedTo = Assign.Agent;
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}
