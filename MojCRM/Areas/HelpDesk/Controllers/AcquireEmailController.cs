using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MojCRM.Areas.HelpDesk.Models.AcquireEmail;
using MojCRM.Areas.HelpDesk.Helpers;
using MojCRM.Areas.HelpDesk.ViewModels;
using OfficeOpenXml;

namespace MojCRM.Areas.HelpDesk.Controllers
{
    public class AcquireEmailController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: HelpDesk/AcquireEmail
        [Authorize]
        public ActionResult Index(AcquireEmailSearchModel model)
        {
            IQueryable<AcquireEmail> list;
            if (User.IsInRole("Administrator") || User.IsInRole("Superadmin") || User.IsInRole("Management"))
            {
                list = _db.AcquireEmails;
            }
            else
            {
                list = _db.AcquireEmails.Where(ae => ae.AcquireEmailStatus != AcquireEmailStatusEnum.Verified && ae.AssignedTo == User.Identity.Name);
            }

            //Search Engine
            if (!String.IsNullOrEmpty(model.CampaignName))
            {
                list = list.Where(x => x.Campaign.CampaignName.Contains(model.CampaignName));
            }
            if (!String.IsNullOrEmpty(model.OrganizationName))
            {
                list = list.Where(x => x.Organization.SubjectName.Contains(model.OrganizationName));
            }
            if (!String.IsNullOrEmpty(model.TelephoneMail))
            {
                list = list.Where(x => x.Organization.MerDeliveryDetail.Telephone.Contains(model.TelephoneMail) || x.Organization.MerDeliveryDetail.AcquiredReceivingInformation.Contains(model.TelephoneMail));
            }
            list = list.Where(x => x.AcquireEmailStatus == model.EmailStatusEnum);

            return View(list.OrderByDescending(x => x.Id));
        }

        public ActionResult Details(int id)
        {
            var entity = _db.AcquireEmails.Find(id);
            var activities = _db.ActivityLogs.Where(al =>
                al.Department == ActivityLog.DepartmentEnum.DatabaseUpdate && al.ReferenceId == id);

            var model = new AcquireEmailViewModel
            {
                Entity = entity,
                Activities = activities
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult LogActivity(int entityId, int identifier)
        {
            var entity = _db.AcquireEmails.Find(entityId);
            switch (identifier)
            {
                case 1:
                    _db.ActivityLogs.Add(new ActivityLog()
                    {
                        Description = "Agent " + User.Identity.Name + " je obavio uspješan poziv za ažuriranje baze korisnika.",
                        User = User.Identity.Name,
                        ReferenceId = entityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.Succall,
                        Department = ActivityLog.DepartmentEnum.DatabaseUpdate,
                        Module = ActivityLog.ModuleEnum.AqcuireEmail,
                        InsertDate = DateTime.Now
                    });
                    entity.LastContactedBy = User.Identity.Name;
                    entity.LastContactDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 2:
                    _db.ActivityLogs.Add(new ActivityLog()
                    {
                        Description = "Agent " + User.Identity.Name + " je pokušao obaviti poziv za ažuriranje baze korisnika.",
                        User = User.Identity.Name,
                        ReferenceId = entityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.Unsuccal,
                        Department = ActivityLog.DepartmentEnum.DatabaseUpdate,
                        Module = ActivityLog.ModuleEnum.AqcuireEmail,
                        InsertDate = DateTime.Now
                    });
                    entity.LastContactedBy = User.Identity.Name;
                    entity.LastContactDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }
            return Json(new { Status = "OK" });
        }


        [HttpPost]
        public JsonResult ChangeStatus(int entityId, int identifier)
        {
            var entity = _db.AcquireEmails.Find(entityId);
            switch (identifier)
            {
                case 1:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Checked;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 2:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 3:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Reviewed;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }
            return Json(new { Status = "OK" });
        }

        [HttpPost]
        public ActionResult ChangeStatusAdmin(int entityId, int identifier)
        {
            var entity = _db.AcquireEmails.Find(entityId);
            switch (identifier)
            {
                case 1:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Checked;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 2:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 3:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Reviewed;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult CheckEntitiesForImport(HttpPostedFileBase file, int campaignId)
        {
            int importedEntities = 0;
            int validEntities = 0;
            List<string> validVATs = new List<string>();
            int invalidEntities = 0;
            List<string> invalidVATs = new List<string>();

            //string filepath = Path.Combine(Server.MapPath("~/ImportFiles"), "ImportAcquireEmail.xls");
            //if(!create)
            //    file.SaveAs(filepath);

            var wb = new ExcelPackage(file.InputStream);
            var ws = wb.Workbook.Worksheets[1];

            for (int i = ws.Dimension.Start.Row; i <= ws.Dimension.End.Row; i++)
            {
                string VAT = ws.Cells[i, 1].Value.ToString();

                if (VAT != "")
                {
                    if (_db.Organizations.Any(o => o.SubjectBusinessUnit == "" && o.VAT == VAT))
                    {
                        validVATs.Add(VAT);
                        ImportEntities(campaignId, VAT);
                        importedEntities++;

                        validEntities++;
                    }
                    else
                    {
                        invalidVATs.Add(VAT);
                        invalidEntities++;
                    }
                }
            }
            
            var model = new AcquireEmailCheckResults
            {
                CampaignId = campaignId,
                ValidEntities = validEntities,
                InvalidEntities = invalidEntities,
                ImportedEntities = importedEntities,
                ValidVATs = validVATs,
                InvalidVATs = invalidVATs
            };

            //if(create)
            //    System.IO.File.Delete(filepath);

            return View(model);
        }

        [HttpPost]
        public void ImportEntities(int campaignId, string VAT)
        {
            if (VAT != "")
            {
                var relatedOrganization = _db.Organizations.First(o => o.SubjectBusinessUnit == "" && o.VAT == VAT);

                if (relatedOrganization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Verified, campaignId);
                }
                else if (relatedOrganization.MerDeliveryDetail.RequiredPostalService)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Checked, campaignId);
                }
                else if (relatedOrganization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified && relatedOrganization.MerDeliveryDetail.RequiredPostalService)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Verified, campaignId);
                }
                else
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.Created, campaignId);
                }
            }
        }

        public ActionResult ExportEntities(int campaignId, int identifier)
        {
            #region OldWay
            //var gv = new GridView();
            //gv.DataSource = GetEntityList(CampaignId);
            //gv.DataBind();
            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=ExportedEntities.xls");
            //Response.ContentType = "application/ms-excel";
            //Response.Charset = "";
            //Response.ContentEncoding = new System.Text.UTF8Encoding();
            //StringWriter objStringWriter = new StringWriter();
            //HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            //gv.RenderControl(objHtmlTextWriter);
            //Response.Output.Write(objStringWriter.ToString());
            //Response.Flush();
            //Response.End();
            #endregion

            int cell = 2;
            IList<AcquireEmailExportModel> results = null;

            switch (identifier)
            {
                case 1:
                    results = GetEntityList(campaignId);
                    break;
                case 2:
                    results = GetReviewedEntityList(campaignId);
                    break;
            }
            var wb = new ExcelPackage();
            var ws = wb.Workbook.Worksheets.Add("Rezultati obrade baze");
            ws.Cells[1, 1].Value = "Naziv kampanje";
            ws.Cells[1, 2].Value = "OIB partnera";
            ws.Cells[1, 3].Value = "Naziv partnera";
            ws.Cells[1, 4].Value = "Informacija o zaprimanju eRačuna";

            foreach (var res in results)
            {
                ws.Cells[cell, 1].Value = res.CampaignName;
                ws.Cells[cell, 2].Value = res.VAT;
                ws.Cells[cell, 3].Value = res.SubjectName;
                ws.Cells[cell, 4].Value = res.AcquiredReceivingInformation;
                cell++;
            }

            while (cell < 16)
            {
                ws.Cells[cell, 1].Value = "";
                cell++;
            }

            return File(wb.GetAsByteArray(), "application/vnd.ms-excel", "Rezultati.xls");
        }

        public ActionResult ExportEntitiesForEmailNotification(int campaignId)
        {
            int cell = 2;
            string campaignName = _db.Campaigns.First(c => c.CampaignId == campaignId).CampaignName;
            IList<AcquireEmailExportForEmailNotificationModel> results = GetEntityListForEmailNotification(campaignId);

            var wb = new ExcelPackage();
            var ws = wb.Workbook.Worksheets.Add("Rezultati obrade baze za tipsku");
            ws.Cells[1, 1].Value = "Informacija o zaprimanju eRačuna";

            foreach (var res in results)
            {
                ws.Cells[cell, 1].Value = res.AcquiredReceivingInformation;
                cell++;
            }

            while (cell < 16)
            {
                ws.Cells[cell, 1].Value = "";
                cell++;
            }

            return File(wb.GetAsByteArray(), "application/vnd.ms-excel", campaignName + ".xls");
        }

        public void CreateEntity(Organizations organization, AcquireEmailStatusEnum status, int campaignId)
        {
            _db.AcquireEmails.Add(new AcquireEmail
            {
                RelatedOrganizationId = organization.MerId,
                RelatedCampaignId = campaignId,
                AcquireEmailStatus = status,
                InsertDate = DateTime.Now
            });
            _db.SaveChanges();
        }

        public JsonResult UpdateEntityStatus(int campaignId)
        {
            int updated = 0;

            var entities = _db.AcquireEmails.Where(ac => ac.RelatedCampaignId == campaignId);

            foreach (var entity in entities)
            {
                if (entity.Organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                else if (entity.Organization.MerDeliveryDetail.RequiredPostalService)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Checked;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                else if (entity.Organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified && entity.Organization.MerDeliveryDetail.RequiredPostalService)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.Verified;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                //else
                //{
                //    entity.AcquireEmailStatus = AcquireEmailStatusEnum.CREATED;
                //    entity.UpdateDate = DateTime.Now;
                //    updated++;
                //}
            }
            _db.SaveChanges();

            return Json(new { updated }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdminAssignEntities(int campaignId, int number, string agent)
        {
            var entites = _db.AcquireEmails.Where(c => c.RelatedCampaignId == campaignId && c.IsAssigned == false).Take(number);

            foreach (var entity in entites)
            {
                entity.IsAssigned = true;
                entity.AssignedTo = agent;
                entity.UpdateDate = DateTime.Now;
            }
            _db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public IList<AcquireEmailExportModel> GetEntityList(int campaignId)
        {
            var entityList = (from ae in _db.AcquireEmails
                              where ae.RelatedCampaignId == campaignId && (ae.AcquireEmailStatus == AcquireEmailStatusEnum.Reviewed || ae.AcquireEmailStatus == AcquireEmailStatusEnum.Verified)
                              select new AcquireEmailExportModel
                              {
                                  CampaignName = ae.Campaign.CampaignName,
                                  VAT = ae.Organization.VAT,
                                  SubjectName = ae.Organization.SubjectName,
                                  AcquiredReceivingInformation = ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation
                              }).ToList();
            return entityList;
        }
        public IList<AcquireEmailExportModel> GetReviewedEntityList(int campaignId)
        {
            var entityList = (from ae in _db.AcquireEmails
                where ae.RelatedCampaignId == campaignId && ae.AcquireEmailStatus == AcquireEmailStatusEnum.Reviewed
                select new AcquireEmailExportModel
                {
                    CampaignName = ae.Campaign.CampaignName,
                    VAT = ae.Organization.VAT,
                    SubjectName = ae.Organization.SubjectName,
                    AcquiredReceivingInformation = ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation
                }).ToList();
            return entityList;
        }

        public IList<AcquireEmailExportForEmailNotificationModel> GetEntityListForEmailNotification(int campaignId)
        {
            var entityList = (from ae in _db.AcquireEmails
                where ae.RelatedCampaignId == campaignId &&
                      ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation.Contains("@")
                      select new AcquireEmailExportForEmailNotificationModel
                      {
                          AcquiredReceivingInformation = ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation
                      }).ToList();
            return entityList;
        }
    }
}