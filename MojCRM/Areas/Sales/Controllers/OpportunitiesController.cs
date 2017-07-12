using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MojCRM.Areas.Campaigns.Models;
using MojCRM.Models;
using MojCRM.Areas.Campaigns.ViewModels;
using MojCRM.Areas.Sales.Models;
using Excel = Microsoft.Office.Interop.Excel;
using MojCRM.Areas.Sales.ViewModels;
using MojCRM.Areas.Sales.Helpers;

namespace MojCRM.Areas.Sales.Controllers
{
    public class OpportunitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Sales/Opportunities
        public ActionResult Index(OpportunitySearchHelper Model)
        {
            var opportunities = from o in db.Opportunities
                                select o;
            if (User.IsInRole("Management") || User.IsInRole("Administrator") || User.IsInRole("Board") || User.IsInRole("Superadmin"))
            {
                //Search Engine
                if (!String.IsNullOrEmpty(Model.Campaign))
                {
                    opportunities = opportunities.Where(op => op.RelatedCampaign.CampaignName == Model.Campaign);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Opportunity))
                {
                    opportunities = opportunities.Where(op => op.OpportunityTitle == Model.Organization);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Organization))
                {
                    opportunities = opportunities.Where(op => op.RelatedOrganization.SubjectName == Model.Organization);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.OpportunityStatus.ToString()))
                {
                    opportunities = opportunities.Where(op => op.OpportunityStatus == Model.OpportunityStatus);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.RejectReason.ToString()))
                {
                    opportunities = opportunities.Where(op => op.RejectReason == Model.RejectReason);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
                if (!String.IsNullOrEmpty(Model.Assigned))
                {
                    if (Model.Assigned == "1")
                    {
                        opportunities = opportunities.Where(op => op.IsAssigned == false);
                        ViewBag.SearchResults = opportunities.Count();
                        ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                    }
                    if (Model.Assigned == "2")
                    {
                        opportunities = opportunities.Where(op => op.IsAssigned == true);
                        ViewBag.SearchResults = opportunities.Count();
                        ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                    }
                }
                if (!String.IsNullOrEmpty(Model.AssignedTo))
                {
                    opportunities = opportunities.Where(op => op.AssignedTo == Model.AssignedTo);
                    ViewBag.SearchResults = opportunities.Count();
                    ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();
                }
            }
            else
            {
                opportunities = opportunities.Where(op => op.AssignedTo == User.Identity.Name);
            }

            ViewBag.SearchResults = opportunities.Count();
            ViewBag.SearchResultsAssigned = opportunities.Where(op => op.IsAssigned == true).Count();

            ViewBag.UsersAssigned = opportunities.Where(op => op.AssignedTo == User.Identity.Name).Count();
            ViewBag.UsersCreated = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.START).Count();
            ViewBag.UsersInContact = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.INCONTACT).Count();
            ViewBag.UsersLead = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.LEAD).Count();
            ViewBag.UsersRejected = opportunities.Where(op => op.AssignedTo == User.Identity.Name && op.OpportunityStatus == Opportunity.OpportunityStatusEnum.REJECTED).Count();

            return View(opportunities.ToList().OrderByDescending(op => op.InsertDate));
        }

        // GET: Sales/Opportunities/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Opportunity opportunity = db.Opportunities.Find(id);
            if (opportunity == null)
            {
                return HttpNotFound();
            }

            var _RelatedSalesContacts = (from c in db.Contacts
                                         where c.Organization.MerId == opportunity.RelatedOrganizationId && c.ContactType == Contact.ContactTypeEnum.SALES
                                         select c).AsEnumerable();
            var _RelatedOpportunityNotes = (from n in db.OpportunityNotes
                                            where n.RelatedOpportunityId == opportunity.OpportunityId
                                            select n).OrderByDescending(n => n.InsertDate).AsEnumerable();
            var _RelatedOpportunityActivities = (from a in db.ActivityLogs
                                                 where a.ReferenceId == id
                                                 select a).OrderByDescending(a => a.InsertDate).AsEnumerable();
            var _RelatedOrganization = (from o in db.Organizations
                                        where o.MerId == opportunity.RelatedOrganizationId
                                        select o).First();
            var _RelatedOrganizationDetail = (from od in db.OrganizationDetails
                                              where od.MerId == opportunity.RelatedOrganizationId
                                              select od).First();
            var _RelatedCampaign = (from c in db.Campaigns
                                    where c.CampaignId == opportunity.RelatedCampaignId
                                    select c).First();
            var _Users = (from u in db.Users
                          select u).AsEnumerable();

            var OpportunityDetails = new OpportunityDetailViewModel()
            {
                OpportunityId = id,
                OpportunityDescription = opportunity.OpportunityDescription,
                OpportunityStatus = opportunity.OpportunityStatusString,
                RejectReasson = opportunity.OpportunityRejectReasonString,
                OrganizationName = _RelatedOrganization.SubjectName,
                OrganizationVAT = _RelatedOrganization.VAT,
                TelephoneNumber = _RelatedOrganizationDetail.TelephoneNumber,
                MobilePhoneNumber = _RelatedOrganizationDetail.MobilePhoneNumber,
                Email = _RelatedOrganizationDetail.EmailAddress,
                ERP = _RelatedOrganizationDetail.ERP,
                NumberOfInvoices = _RelatedOrganizationDetail.NumberOfInvoices,
                RelatedCampaignName = _RelatedCampaign.CampaignName,
                IsAssigned = opportunity.IsAssigned,
                AssignedTo = opportunity.AssignedTo,
                LastContactedDate = opportunity.LastContactDate,
                LastContactedBy = opportunity.LastContactedBy,
                RelatedSalesContacts = _RelatedSalesContacts,
                RelatedOpportunityNotes = _RelatedOpportunityNotes,
                RelatedOpportunityActivities = _RelatedOpportunityActivities,
                Users = _Users
            };

            return View(OpportunityDetails);
        }

        // POST: Sales/Opportunities/AddNote
        [HttpPost]
        public ActionResult AddNote(OpportunityNoteHelper Model)
        {
            var _RelatedOpportunity = (from o in db.Opportunities
                                       where o.OpportunityId == Model.RelatedOpportunityId
                                       select o).First();

            _RelatedOpportunity.LastContactDate = DateTime.Now;
            _RelatedOpportunity.LastContactedBy = User.Identity.Name;
            db.OpportunityNotes.Add(new OpportunityNote
            {
                RelatedOpportunityId = Model.RelatedOpportunityId,
                User = User.Identity.Name,
                Note = Model.Note,
                InsertDate = DateTime.Now,
                Contact = Model.Contact
            });
            db.SaveChanges();

            switch (Model.Identifier)
            {
                case 1:
                    db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = User.Identity.Name + " je obavio uspješan poziv vezan za prodajnu priliku: " + _RelatedOpportunity.OpportunityTitle,
                        User = User.Identity.Name,
                        ReferenceId = Model.RelatedOpportunityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.SUCCALL,
                        Department = ActivityLog.DepartmentEnum.Sales,
                        InsertDate = DateTime.Now
                    });
                    db.SaveChanges();
                    break;
                case 2:
                    db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = Model.User + " je obavio kraći informativni poziv vezano za prodajnu priliku: " + _RelatedOpportunity.OpportunityTitle,
                        User = Model.User,
                        ReferenceId = Model.RelatedOpportunityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.SUCCALSHORT,
                        Department = ActivityLog.DepartmentEnum.Sales,
                        InsertDate = DateTime.Now,
                    });
                    db.SaveChanges();
                    break;
                case 3:
                    db.ActivityLogs.Add(new ActivityLog
                    {
                        Description = Model.User + " je pokušao obaviti telefonski poziv vezano za prodajnu priliku: " + _RelatedOpportunity.OpportunityTitle,
                        User = Model.User,
                        ReferenceId = Model.RelatedOpportunityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.UNSUCCAL,
                        Department = ActivityLog.DepartmentEnum.Sales,
                        InsertDate = DateTime.Now,
                    });
                    db.SaveChanges();
                    break;
            }

            return RedirectToAction("Details", new { id = Model.RelatedOpportunityId });
        }

        // POST: Sales/Opportunities/EditNote
        [HttpPost]
        [Authorize]
        public ActionResult EditNote(OpportunityNoteHelper Model)
        {
            var NoteForEdit = (from n in db.OpportunityNotes
                               where n.Id == Model.NoteId
                               select n).First();

            NoteForEdit.Note = Model.Note;
            NoteForEdit.Contact = Model.Contact;
            NoteForEdit.UpdateDate = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = Model.RelatedOpportunityId });
        }

        // POST: Sales/Opportunities/DeleteNote
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Superadmin")]
        public ActionResult DeleteNote(OpportunityNoteHelper Model)
        {
            OpportunityNote opportunityNote = db.OpportunityNotes.Find(Model.NoteId);
            db.OpportunityNotes.Remove(opportunityNote);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = Model.RelatedOpportunityId });
        }

        public void LogEmail(OpportunityNoteHelper Model)
        {
            var _RelatedOpportunity = (from o in db.Opportunities
                                       where o.OpportunityId == Model.RelatedOpportunityId
                                       select o).First();

            _RelatedOpportunity.LastContactDate = DateTime.Now;
            _RelatedOpportunity.LastContactedBy = Model.User;
            db.ActivityLogs.Add(new ActivityLog
            {
                Description = Model.User + " je poslao e-mail na adresu: " + Model.Email + " na temu prezentacije usluge u sklopu prodajne prilike: " + _RelatedOpportunity.OpportunityTitle,
                User = Model.User,
                ReferenceId = Model.RelatedOpportunityId,
                ActivityType = ActivityLog.ActivityTypeEnum.EMAIL,
                Department = ActivityLog.DepartmentEnum.Sales,
                InsertDate = DateTime.Now,
            });
            db.SaveChanges();
        }

        public ActionResult Assign(OpportunityAssignHelper Model)
        {
            var opportunity = (from o in db.Opportunities
                               where o.OpportunityId == Model.RelatedOpportunityId
                               select o).First();
            opportunity.IsAssigned = true;
            opportunity.AssignedTo = Model.AssignedTo;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Reassign(OpportunityAssignHelper Model)
        {
            var opportunity = (from o in db.Opportunities
                               where o.OpportunityId == Model.RelatedOpportunityId
                               select o).First();
            if (Model.Unassign == true)
            {
                opportunity.IsAssigned = false;
                opportunity.AssignedTo = String.Empty;
                db.SaveChanges();
            }
            else
            {
                opportunity.AssignedTo = Model.AssignedTo;
                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Sales/Opportunities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Opportunity opportunity = db.Opportunities.Find(id);
            if (opportunity == null)
            {
                return HttpNotFound();
            }
            return View(opportunity);
        }

        // POST: Sales/Opportunities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Opportunity _Opportunity)
        {
            var Model = new OpportunityEditHelper()
            {
                OpportunityId = _Opportunity.OpportunityId,
                OpportunityDescription = _Opportunity.OpportunityDescription,
                OpportunityStatus = _Opportunity.OpportunityStatus,
                RejectReason = _Opportunity.RejectReason
            };
            if (ModelState.IsValid)
            {
                var opportunity = db.Opportunities.Find(Model.OpportunityId);
                opportunity.OpportunityDescription = Model.OpportunityDescription;
                opportunity.OpportunityStatus = Model.OpportunityStatus;
                opportunity.RejectReason = Model.RejectReason;
                opportunity.UpdateDate = DateTime.Now;
                opportunity.LastUpdatedBy = User.Identity.Name;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Model);
        }

        public void ImportOpportunities()
        {
            //create a instance for the Excel object  
            Excel.Application oExcel = new Excel.Application();

            //specify the file name where its actually exist  
            string filepath = @"C:\Temp\Test.xlsx";

            //pass that to workbook object  
            Excel.Workbook WB = oExcel.Workbooks.Open(filepath);


            // statement get the workbookname  
            string ExcelWorkbookname = WB.Name;

            // statement get the worksheet count  
            int worksheetcount = WB.Worksheets.Count;

            Excel.Worksheet wks = (Excel.Worksheet)WB.Worksheets[1];

            // statement get the firstworksheetname  

            string firstworksheetname = wks.Name;

            //statement get the first cell value  
            var firstcellvalue = ((Excel.Range)wks.Cells[1, 1]).Value;
        }
    }
}