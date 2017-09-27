using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static MojCRM.Areas.HelpDesk.Models.AcquireEmail;
using ExcelOut = ExcelLibrary.SpreadSheet;
using MojCRM.Areas.HelpDesk.Helpers;

namespace MojCRM.Areas.HelpDesk.Controllers
{
    public class AcquireEmailController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: HelpDesk/AcquireEmail
        [Authorize]
        public ActionResult Index()
        {
            var list = db.AcquireEmails.Where(ae => ae.AcquireEmailStatus != Models.AcquireEmail.AcquireEmailStatusEnum.VERIFIED);
            return View(list.OrderByDescending(x => x.Id));
        }

        public ActionResult Details(int id)
        {
            var entity = db.AcquireEmails.Find(id);

            return View(entity);
        }

        [HttpPost]
        public JsonResult LogActivity(int EntityId, int Identifier)
        {
            var entity = db.AcquireEmails.Find(EntityId);
            switch (Identifier)
            {
                case 1:
                    db.ActivityLogs.Add(new ActivityLog()
                    {
                        Description = "Agent " + User.Identity.Name + " je obavio uspješan poziv za ažuriranje baze korisnika.",
                        User = User.Identity.Name,
                        ReferenceId = EntityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.SUCCALL,
                        Department = ActivityLog.DepartmentEnum.DatabaseUpdate,
                        Module = ActivityLog.ModuleEnum.AqcuireEmail,
                        InsertDate = DateTime.Now
                    });
                    entity.LastContactedBy = User.Identity.Name;
                    entity.LastContactDate = DateTime.Now;
                    db.SaveChanges();
                    break;
                case 2:
                    db.ActivityLogs.Add(new ActivityLog()
                    {
                        Description = "Agent " + User.Identity.Name + " je pokušao obaviti poziv za ažuriranje baze korisnika.",
                        User = User.Identity.Name,
                        ReferenceId = EntityId,
                        ActivityType = ActivityLog.ActivityTypeEnum.UNSUCCAL,
                        Department = ActivityLog.DepartmentEnum.DatabaseUpdate,
                        Module = ActivityLog.ModuleEnum.AqcuireEmail,
                        InsertDate = DateTime.Now
                    });
                    entity.LastContactedBy = User.Identity.Name;
                    entity.LastContactDate = DateTime.Now;
                    db.SaveChanges();
                    break;
            }
            return Json(new { Status = "OK" });
        }


        [HttpPost]
        public JsonResult ChangeStatus(int EntityId, int Identifier)
        {
            var entity = db.AcquireEmails.Find(EntityId);
            switch (Identifier)
            {
                case 1:
                    entity.AcquireEmailStatus = Models.AcquireEmail.AcquireEmailStatusEnum.CHECKED;
                    entity.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                    break;
                case 2:
                    entity.AcquireEmailStatus = Models.AcquireEmail.AcquireEmailStatusEnum.VERIFIED;
                    entity.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                    break;
            }
            return Json(new { Status = "OK" });
        }

        public ActionResult CheckEntitiesForImport(int campaignId, bool create = false)
        {
            int importedEntities = 0;
            int validEntities = 0;
            List<string> validVATs = new List<string>();
            int invalidEntities = 0;
            List<string> invalidVATs = new List<string>();

            string filepath = @"C:\MojCRM\ImportAcquireEmail.xls";

            ExcelOut.Workbook wb = ExcelOut.Workbook.Load(filepath);
            ExcelOut.Worksheet ws = wb.Worksheets[0];

            for (int rowIndex = ws.Cells.FirstRowIndex; rowIndex <= ws.Cells.LastRowIndex; rowIndex++)
            {
                ExcelOut.Row row = ws.Cells.GetRow(rowIndex);

                for (int colIndex = row.FirstColIndex; colIndex <= row.LastColIndex; colIndex++)
                {
                    ExcelOut.Cell cell = row.GetCell(colIndex);

                    string VAT = cell.StringValue;

                    if (VAT != "")
                    {
                        if (db.Organizations.Any(o => o.SubjectBusinessUnit == "" && o.VAT == VAT))
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

            return View(model);
        }

        [HttpPost]
        public void ImportEntities(int campaignId, string VAT)
        {
            if (VAT != "")
            {
                var relatedOrganization = db.Organizations.First(o => o.SubjectBusinessUnit == "" && o.VAT == VAT);

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

            string savePath = @"C:\MojCRM\ExportAcquireEmail.xls";
            int cell = 1;
            var results = GetEntityList(campaignId);
            ExcelOut.Workbook wb = new ExcelOut.Workbook();
            ExcelOut.Worksheet ws =
                new ExcelOut.Worksheet("Rezultati obrade baze")
                {
                    Cells =
                    {
                        [0, 0] = new ExcelOut.Cell("Naziv kampanje"),
                        [0, 1] = new ExcelOut.Cell("OIB partnera"),
                        [0, 2] = new ExcelOut.Cell("Naziv partnera"),
                        [0, 3] = new ExcelOut.Cell("Informacija o zaprimanju eRačuna")
                    }
                };


            foreach (var res in results)
            {
                ws.Cells[cell, 0] = new ExcelOut.Cell(res.CampaignName);
                ws.Cells[cell, 1] = new ExcelOut.Cell(res.VAT);
                ws.Cells[cell, 2] = new ExcelOut.Cell(res.SubjectName);
                ws.Cells[cell, 3] = new ExcelOut.Cell(res.AcquiredReceivingInformation);
                cell++;
            }

            wb.Worksheets.Add(ws);
            wb.Save(savePath);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public void CreateEntity(Organizations organization, AcquireEmailStatusEnum status, int campaignId)
        {
            db.AcquireEmails.Add(new AcquireEmail
            {
                RelatedOrganizationId = organization.MerId,
                RelatedCampaignId = campaignId,
                AcquireEmailStatus = status,
                InsertDate = DateTime.Now
            });
            db.SaveChanges();
        }

        public IList<AcquireEmailExportModel> GetEntityList(int campaignId)
        {
            var entityList = (from ae in db.AcquireEmails
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