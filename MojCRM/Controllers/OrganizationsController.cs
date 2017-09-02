using MojCRM.Models;
using MojCRM.Helpers;
using MojCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MojCRM.Controllers
{
    public class OrganizationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Organizations
        public ActionResult Index(string Organization)
        {
            var Organizations = from o in db.Organizations
                                where o.SubjectBusinessUnit == String.Empty
                                select o;

            if (!String.IsNullOrEmpty(Organization))
            {
                Organizations = Organizations.Where(o => o.SubjectName.Contains(Organization) || o.VAT.Contains(Organization));
            }

            return View(Organizations.OrderBy(o => o.MerId));
        }

        // GET: Organizations/Details/5
        public ActionResult Details(int id)
        {
            var organization = db.Organizations.Find(id);
            var model = new OrganizationDetailsViewModel()
            {
                Organization = organization,
                OrganizationDetails = db.OrganizationDetails.Where(od => od.MerId == id).First(),
                MerDeliveryDetails = db.MerDeliveryDetails.Where(mdd => mdd.MerId == id).First(),
                OrganizationBusinessUnits = db.Organizations.Where(o => o.VAT == organization.VAT && o.SubjectBusinessUnit != ""),
                Contacts = db.Contacts.Where(c => c.OrganizationId == id),
                Opportunities = db.Opportunities.Where(op => op.RelatedOrganizationId == id),
                OpportunitiesCount = db.Opportunities.Where(op => op.RelatedOrganizationId == id).Count(),
                Leads = db.Leads.Where(l => l.RelatedOrganizationId == id),
                LeadsCount = db.Leads.Where(l => l.RelatedOrganizationId == id).Count(),
                TicketsAsReceiver = db.DeliveryTicketModels.Where(t => t.ReceiverId == id).OrderByDescending(t => t.SentDate),
                TicketsAsReceiverCount = db.DeliveryTicketModels.Where(t => t.ReceiverId == id).OrderByDescending(t => t.SentDate).Count(),
                TicketsAsSender = db.DeliveryTicketModels.Where(t => t.SenderId == id).OrderByDescending(t => t.SentDate),
                TicketsAsSenderCount = db.DeliveryTicketModels.Where(t => t.SenderId == id).OrderByDescending(t => t.SentDate).Count(),
                Attributes = db.OrganizationAttributes.Where(a => a.OrganizationId == id).OrderBy(a => a.AttributeClass)
            };

            return View(model);
        }

        // GET: Organizations/GetOrganizations
        public JsonResult GetOrganizations(Guid user)
        {
            var Credentials = new { MerUser = "", MerPass = "" };
            if (String.IsNullOrEmpty(user.ToString()))
            {
                Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }
            else
            {
                Credentials = (from u in db.Users
                               where u.Id == user.ToString()
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            }

            var ReferencedId = (from o in db.Organizations
                               orderby o.MerId descending
                               select o.MerId).First();
            int CreatedCompanies = 0;
            var Response = new MerGetSubjektDataResponse()
            {
                Id = 238,
                Naziv = "Test Klising d.o.o."
            };
            ReferencedId++;
            try
            {
                while (Response != null)
                {
                    MerApiGetSubjekt Request = new MerApiGetSubjekt()
                    {
                        Id = Credentials.MerUser,
                        Pass = Credentials.MerPass,
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = ReferencedId.ToString()
                    };

                    string MerRequest = JsonConvert.SerializeObject(Request);

                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                        var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                        _Response = _Response.Replace("[", "").Replace("]", "");
                        MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);
                        if (Result == null)
                        {
                            break;
                        }
                        else
                        {
                            db.Organizations.Add(new Organizations
                            {
                                MerId = Result.Id,
                                SubjectName = Result.Naziv,
                                SubjectBusinessUnit = Result.PoslovnaJedinica,
                                VAT = Result.Oib,
                                FirstReceived = Result.FirstReceived,
                                FirstSent = Result.FirstSent,
                                ServiceProvider = (Organizations.ServiceProviderEnum)Result.ServiceProviderId,
                                InsertDate = DateTime.Now
                            });
                            db.MerDeliveryDetails.Add(new MerDeliveryDetails
                            {
                                MerId = Result.Id,
                                TotalSent = Result.TotalSent,
                                TotalReceived = Result.TotalReceived
                            });
                            db.OrganizationDetails.Add(new OrganizationDetail
                            {
                                MerId = Result.Id,
                                MainAddress = Result.Adresa,
                                MainPostalCode = Int32.Parse(Result.Mjesto.Substring(0, 5).Trim()),
                                MainCity = Result.Mjesto.Substring(6).Trim(),
                                OrganizationGroup = OrganizationGroupEnum.Nema
                            });
                            db.SaveChanges();
                            Result = Response;
                        }
                    }
                    ReferencedId++;
                    CreatedCompanies++;
                }
            }
            catch (NullReferenceException e)
            {
                db.LogError.Add(new LogError
                {
                    Method = @"Organizations - GetOrganizations",
                    Parameters = ReferencedId.ToString(),
                    Message = @"Greška kod preuzimanja podataka o tvrtki",
                    InnerException = e.InnerException.ToString(),
                    Request = e.Source,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now
                });
                db.SaveChanges();
            }

            return Json(new { Status = "OK", CreatedCompanies = CreatedCompanies }, JsonRequestBehavior.AllowGet);
        }

        // GET: Organization/UpdateOrganization/1
        public ActionResult UpdateOrganization(int MerId)
        {
            var Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            var Organization = db.Organizations.Find(MerId);

            using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
            {
                MerApiGetSubjekt Request = new MerApiGetSubjekt()
                {
                    Id = Credentials.MerUser,
                    Pass = Credentials.MerPass,
                    Oib = "99999999927",
                    PJ = "",
                    SoftwareId = "MojCRM-001",
                    SubjektPJ = Organization.MerId.ToString()
                };

                string MerRequest = JsonConvert.SerializeObject(Request);

                Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                _Response = _Response.Replace("[", "").Replace("]", "");
                MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);

                string postalCode = Result.Mjesto.Substring(0, 5).Trim();
                string mainCity = Result.Mjesto.Substring(6).Trim();

                Organization.SubjectName = Result.Naziv;
                Organization.FirstReceived = Result.FirstReceived;
                Organization.FirstSent = Result.FirstSent;
                Organization.ServiceProvider = (Organizations.ServiceProviderEnum)Result.ServiceProviderId;
                Organization.UpdateDate = DateTime.Now;
                Organization.LastUpdatedBy = User.Identity.Name;
                Organization.MerUpdateDate = DateTime.Now;
                Organization.OrganizationDetail.MainAddress = Result.Adresa;
                Organization.OrganizationDetail.MainPostalCode = Int32.Parse(postalCode);
                Organization.OrganizationDetail.MainCity = mainCity;
                Organization.MerDeliveryDetail.TotalSent = Result.TotalSent;
                Organization.MerDeliveryDetail.TotalReceived = Result.TotalReceived;
            }
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Organizations/UpdateOrganizations
        public void UpdateOrganizations()
        {
            var Credentials = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { MerUser = u.MerUserUsername, MerPass = u.MerUserPassword }).First();
            var Organizations = from o in db.Organizations
                                 select o;


                foreach (var Organization in Organizations)
                {
                MerApiGetSubjekt Request = new MerApiGetSubjekt()
                    {
                        Id = Credentials.MerUser,
                        Pass = Credentials.MerPass,
                        Oib = "99999999927",
                        PJ = "",
                        SoftwareId = "MojCRM-001",
                        SubjektPJ = Organization.MerId.ToString()
                    };

                    string MerRequest = JsonConvert.SerializeObject(Request);

                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                    Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                    _Response = _Response.Replace("[", "").Replace("]", "");
                    MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);

                    string postalCode = Result.Mjesto.Substring(0, 5).Trim();
                    string mainCity = Result.Mjesto.Substring(6).Trim();

                    Organization.SubjectName = Result.Naziv;
                    Organization.FirstReceived = Result.FirstReceived;
                    Organization.FirstSent = Result.FirstSent;
                    Organization.ServiceProvider = (Organizations.ServiceProviderEnum)Result.ServiceProviderId;
                    Organization.UpdateDate = DateTime.Now;
                    Organization.LastUpdatedBy = User.Identity.Name;
                    Organization.MerUpdateDate = DateTime.Now;
                    Organization.OrganizationDetail.MainAddress = Result.Adresa;
                    Organization.OrganizationDetail.MainPostalCode = Int32.Parse(postalCode);
                    Organization.OrganizationDetail.MainCity = mainCity;
                    Organization.MerDeliveryDetail.TotalSent = Result.TotalSent;
                    Organization.MerDeliveryDetail.TotalReceived = Result.TotalReceived;
                }
            }
            db.SaveChanges();
        }

        // POST: Organizations/EditImportantComment
        [HttpPost]
        public ActionResult EditImportantComment(string Comment, string ReceiverId)
        {
            int ReceiverIdInt = Int32.Parse(ReceiverId);

            var DetailForEdit = (from dd in db.MerDeliveryDetails
                                   where dd.MerId == ReceiverIdInt
                                   select dd).First();

            DetailForEdit.ImportantComments = Comment;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Organizations/EditOrganizationDetails
        public ActionResult EditOrganizationDetails(EditOrganizationDetails Model)
        {
            var organization = db.OrganizationDetails.Find(Model.MerId);
            string LogString = "Agent " + User.Identity.Name + " je napravio izmjene na subjektu: "
                + organization.Organization.SubjectName + ". Izmjenjeni su:";

            if (!String.IsNullOrEmpty(Model.TelephoneNumber))
            {
                if (!String.Equals(Model.TelephoneNumber, organization.TelephoneNumber))
                    LogString += " - broj telefona iz " + organization.TelephoneNumber + " u " + Model.TelephoneNumber;
                organization.TelephoneNumber = Model.TelephoneNumber;
            }
            if (!String.IsNullOrEmpty(Model.MobilePhoneNumber))
            {
                if (!String.Equals(Model.MobilePhoneNumber, organization.MobilePhoneNumber))
                    LogString += " - broj mobitela iz " + organization.MobilePhoneNumber + " u " + Model.MobilePhoneNumber;
                organization.MobilePhoneNumber = Model.MobilePhoneNumber;
            }
            if (!String.IsNullOrEmpty(Model.EmailAddress))
            {
                if (!String.Equals(Model.EmailAddress, organization.EmailAddress))
                    LogString += " - e-mail adresa iz " + organization.EmailAddress + " u " + Model.EmailAddress;
                organization.EmailAddress = Model.EmailAddress;
            }
            if (!String.IsNullOrEmpty(Model.ERP))
            {
                if (!String.Equals(Model.ERP, organization.ERP))
                    LogString += " - ERP iz " + organization.ERP + " u " + Model.ERP;
                organization.ERP = Model.ERP;
            }
            if (!String.IsNullOrEmpty(Model.NumberOfInvoicesSent))
            {
                if (!String.Equals(Model.NumberOfInvoicesSent, organization.NumberOfInvoicesSent))
                    LogString += " - broj IRA iz " + organization.NumberOfInvoicesSent + " u " + Model.NumberOfInvoicesSent;
                organization.NumberOfInvoicesSent = Model.NumberOfInvoicesSent;
            }
            if (!String.IsNullOrEmpty(Model.NumberOfInvoicesReceived))
            {
                if (!String.Equals(Model.NumberOfInvoicesReceived, organization.NumberOfInvoicesReceived))
                    LogString += " - broj URA iz " + organization.NumberOfInvoicesReceived + " u " + Model.NumberOfInvoicesReceived;
                organization.NumberOfInvoicesReceived = Model.NumberOfInvoicesReceived;
            }
            if (!String.IsNullOrEmpty(Model.CorrespondenceAddress))
            {
                if (!String.Equals(Model.CorrespondenceAddress, organization.CorrespondenceAddress))
                    LogString += " - adresa za dostavu iz " + organization.CorrespondenceAddress + " u " + Model.CorrespondenceAddress;
                organization.CorrespondenceAddress = Model.CorrespondenceAddress;
            }
            if (!String.IsNullOrEmpty(Model.CorrespondenceCity))
            {
                if (!String.Equals(Model.CorrespondenceCity, organization.CorrespondenceCity))
                    LogString += " - grad/mjesto za dostavu iz " + organization.CorrespondenceCity + " u " + Model.CorrespondenceCity;
                organization.CorrespondenceCity = Model.CorrespondenceCity;
            }
            if (Model.CorrespondencePostalCode != 0)
            {
                if (Model.CorrespondencePostalCode != organization.CorrespondencePostalCode)
                    LogString += " - poštanski broj iz " + organization.CorrespondencePostalCode + " u " + Model.CorrespondencePostalCode;
                organization.CorrespondencePostalCode = Model.CorrespondencePostalCode;
            }
            LogString += ".";
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;

            LogActivity(LogString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.ORGANIZATIONUPDATE);

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // POST: Organizations/EditImportantOrganizationInfo
        [Authorize(Roles = "Superadmin")]
        public ActionResult EditImportantOrganizationInfo(EditImportantOrganizationInfo Model)
        {
            var organization = db.Organizations.Find(Model.MerId);

            string LogString = "Agent " + User.Identity.Name + " je napravio izmjene na subjektu: "
                + organization.SubjectName + ". Izmjenjeni su:";

            if (Model.LegalForm != null)
            {
                if (Model.LegalForm != organization.LegalForm)
                    LogString += " - pravni oblik iz " + organization.LegalForm + " u " + Model.LegalForm;
                organization.LegalForm = (Organizations.LegalFormEnum)Model.LegalForm;
            }
            if (Model.OrganizationGroup != null)
            {
                if (Model.OrganizationGroup != organization.OrganizationDetail.OrganizationGroup)
                    LogString += " - članstvo u grupaciji iz " + organization.OrganizationDetail.OrganizationGroup + " u " + Model.OrganizationGroup;
                organization.OrganizationDetail.OrganizationGroup = (OrganizationGroupEnum)Model.OrganizationGroup;
            }
            if (Model.ServiceProvider != null)
            {
                if (Model.ServiceProvider != organization.ServiceProvider)
                    LogString += " - informacijski posrednik iz " + organization.ServiceProvider + " u " + Model.ServiceProvider;
                organization.ServiceProvider = (Organizations.ServiceProviderEnum)Model.ServiceProvider;
            }
            if (Model.LegalStatus != null)
            {
                if (Model.LegalStatus == 0)
                {
                    var temp = false;
                    if (temp != organization.IsActive)
                        LogString += " - pravni status iz aktivnog u brisano";
                    organization.IsActive = false;
                }
                if (Model.LegalStatus == 1)
                {
                    var temp = true;
                    if (temp != organization.IsActive)
                        LogString += " - pravni status iz brisanog u aktivno";
                    organization.IsActive = true;
                }
            }
            LogString += ".";
            organization.UpdateDate = DateTime.Now;
            organization.LastUpdatedBy = User.Identity.Name;

            LogActivity(LogString, User.Identity.Name, organization.MerId, ActivityLog.ActivityTypeEnum.ORGANIZATIONUPDATE);

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Organizations/AddAttribute
        public ActionResult AddAttribute()
        {
            var model = new AddOrganizationAttributeViewModel();
            return View(model);
        }

        // POST: Organizations/AddAttribute
        [HttpPost]
        [Authorize(Roles = "Superadmin")]
        public ActionResult AddAttribute(AddOrganizationAttribute Model)
        {
            db.OrganizationAttributes.Add(new OrganizationAttribute
            {
                OrganizationId = Model.MerId,
                AttributeClass = Model.AttributeClass,
                AttributeType = Model.AttributeType,
                IsActive = true,
                AssignedBy = User.Identity.Name,
                InsertDate = DateTime.Now
            });
            db.SaveChanges();
            return RedirectToAction("Details", new { id = Model.MerId });
        }

        // POST: Organizations/CopyMainAddress
        [HttpPost]
        public JsonResult CopyMainAddress(EditOrganizationDetails Model)
        {
            var organization = db.OrganizationDetails.Find(Model.MerId);

            organization.CorrespondenceAddress = Model.MainAddress;
            organization.CorrespondencePostalCode = Model.MainPostalCode;
            organization.CorrespondenceCity = Model.MainCity;
            organization.Organization.UpdateDate = DateTime.Now;
            organization.Organization.LastUpdatedBy = User.Identity.Name;
            db.SaveChanges();

            return Json(new { Status = "OK"});
        }

        public void LogActivity(string ActivityDescription, string User, int ActivityReferenceId, ActivityLog.ActivityTypeEnum ActivityType)
        {
            db.ActivityLogs.Add(new ActivityLog
            {
                Description = ActivityDescription,
                User = User,
                ReferenceId = ActivityReferenceId,
                ActivityType = ActivityType,
                Department = ActivityLog.DepartmentEnum.MojCRM,
                Module = ActivityLog.ModuleEnum.Organizations,
                InsertDate = DateTime.Now
            });
            db.SaveChanges();
        }

        public JsonResult AutocompleteOrganization(string query)
        {
            try
            {
                var organizations = db.Organizations.Where(o =>
                o.SubjectName.StartsWith(query) ||
                o.VAT.StartsWith(query))
                .Select(o => new { Organization = o.SubjectName + " - " + o.VAT, MerId = o.MerId})
                .Take(10)
                .ToList();

                //var _model = new List<OrganizationSearch>();
                //foreach (var org in organizations)
                //    _model.Add(new OrganizationSearch
                //    {
                //        MerId = org.MerId,
                //        OrganizationName = org.SubjectName + " - " + org.VAT
                //    });
                //var model = JsonConvert.SerializeObject(_model);
                return Json(organizations, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                db.LogError.Add(new LogError
                {
                    Method = @"Organizations - AutocompleteOrganization",
                    Parameters = query,
                    Message = @"Dogodila se greška prilikom pretraživanja tvrtki. Opis: " + ex.Message,
                    User = User.Identity.Name,
                    InsertDate = DateTime.Now
                });
                db.SaveChanges();
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}