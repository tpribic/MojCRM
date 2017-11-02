using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static MojCRM.Areas.HelpDesk.Models.AcquireEmail;
using MojCRM.Areas.HelpDesk.Helpers;
using OfficeOpenXml;

namespace MojCRM.Areas.HelpDesk.Controllers
{
    public class AcquireEmailController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        // GET: HelpDesk/AcquireEmail
        [Authorize]
        public ActionResult Index()
        {
            var list = _db.AcquireEmails.Where(ae => ae.AcquireEmailStatus != AcquireEmailStatusEnum.VERIFIED);
            return View(list.OrderByDescending(x => x.Id));
        }

        public ActionResult Details(int id)
        {
            var entity = _db.AcquireEmails.Find(id);

            return View(entity);
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
                        ActivityType = ActivityLog.ActivityTypeEnum.SUCCALL,
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
                        ActivityType = ActivityLog.ActivityTypeEnum.UNSUCCAL,
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
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.CHECKED;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
                case 2:
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.VERIFIED;
                    entity.UpdateDate = DateTime.Now;
                    _db.SaveChanges();
                    break;
            }
            return Json(new { Status = "OK" });
        }

        [HttpPost]
        public ActionResult CheckEntitiesForImport(HttpPostedFileBase file, int campaignId, bool create = false)
        {
            int importedEntities = 0;
            int validEntities = 0;
            List<string> validVATs = new List<string>();
            int invalidEntities = 0;
            List<string> invalidVATs = new List<string>();

            string filepath = Path.Combine(Server.MapPath("~/ImportFiles"), "ImportAcquireEmail.xls");
            if(!create)
                file.SaveAs(filepath);

            var wb = new ExcelPackage(new FileInfo(filepath));
            var ws = wb.Workbook.Worksheets[1];

            for (int i = ws.Dimension.Start.Row; i <= ws.Dimension.End.Row; i++)
            {
                string VAT = ws.Cells[i, 1].Value.ToString();

                if (VAT != "")
                {
                    if (_db.Organizations.Any(o => o.SubjectBusinessUnit == "" && o.VAT == VAT))
                    {
                        validVATs.Add(VAT);
                        if (create)
                        {
                            ImportEntities(campaignId, VAT);
                            importedEntities++;
                        }

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

            if(create)
                System.IO.File.Delete(filepath);

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
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.VERIFIED, campaignId);
                }
                else if (relatedOrganization.MerDeliveryDetail.RequiredPostalService)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.CHECKED, campaignId);
                }
                else if (relatedOrganization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified && relatedOrganization.MerDeliveryDetail.RequiredPostalService)
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.VERIFIED, campaignId);
                }
                else
                {
                    CreateEntity(relatedOrganization, AcquireEmailStatusEnum.CREATED, campaignId);
                }
            }
        }

        public ActionResult ExportEntities(int campaignId)
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
            var results = GetEntityList(campaignId);
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
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.VERIFIED;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                else if (entity.Organization.MerDeliveryDetail.RequiredPostalService)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.CHECKED;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                else if (entity.Organization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified && entity.Organization.MerDeliveryDetail.RequiredPostalService)
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.VERIFIED;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
                else
                {
                    entity.AcquireEmailStatus = AcquireEmailStatusEnum.CREATED;
                    entity.UpdateDate = DateTime.Now;
                    updated++;
                }
            }
            _db.SaveChanges();

            return Json(new { updated }, JsonRequestBehavior.AllowGet);
        }

        public IList<AcquireEmailExportModel> GetEntityList(int campaignId)
        {
            var entityList = (from ae in _db.AcquireEmails
                              where ae.RelatedCampaignId == campaignId && ae.AcquireEmailStatus == AcquireEmailStatusEnum.VERIFIED
                              select new AcquireEmailExportModel
                              {
                                  CampaignName = ae.Campaign.CampaignName,
                                  VAT = ae.Organization.VAT,
                                  SubjectName = ae.Organization.SubjectName,
                                  AcquiredReceivingInformation = ae.Organization.MerDeliveryDetail.AcquiredReceivingInformation
                              }).ToList();
            return entityList;
        }
    }
}