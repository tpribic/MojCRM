using Microsoft.Office.Interop.Excel;
using MojCRM.Areas.HelpDesk.Models;
using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using static MojCRM.Areas.HelpDesk.Models.AcquireEmail;
using Excel = Microsoft.Office.Interop.Excel;
using MojCRM.Areas.HelpDesk.Helpers;
using System.Web.UI.WebControls;
using System.Web.UI;

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

        public JsonResult ImportEntities(int CampaignId)
        {
            int ImportedEntities = 0;

            Application oExcel = new Application();
            string filepath = @"C:\MojCRM\ImportAcquireEmail.xlsx";
            Workbook WB = oExcel.Workbooks.Open(filepath);
            Worksheet wks = (Excel.Worksheet)WB.Worksheets[1];

            Range firstColumn = wks.UsedRange.Columns[1];
            Array VATTemp = (Array)firstColumn.Cells.Value;
            string[] VATs = VATTemp.OfType<object>().Select(o => o.ToString()).ToArray();

            foreach (var VAT in VATs)
            {
                if (VAT != "")
                {
                    var relatedOrganization = db.Organizations.Where(o => o.SubjectBusinessUnit == "" && o.VAT == VAT).First();

                    if (relatedOrganization.MerDeliveryDetail.AcquiredReceivingInformationIsVerified)
                    {
                        CreateEntity(relatedOrganization, AcquireEmailStatusEnum.VERIFIED, CampaignId);
                        ImportedEntities++;
                    }
                    else if (relatedOrganization.MerDeliveryDetail.RequiredPostalService)
                    {
                        CreateEntity(relatedOrganization, AcquireEmailStatusEnum.CHECKED, CampaignId);
                        ImportedEntities++;
                    }
                    else
                    {
                        CreateEntity(relatedOrganization, AcquireEmailStatusEnum.CREATED, CampaignId);
                        ImportedEntities++;
                    }
                }
                else
                    break;
            }

            return Json(new { NumberOfImportedEntites = ImportedEntities }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportEntities(int CampaignId)
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



            return Redirect(Request.UrlReferrer.ToString());
        }

        public void CreateEntity(Organizations Organization, AcquireEmailStatusEnum Status, int CampaignId)
        {
            db.AcquireEmails.Add(new AcquireEmail
            {
                RelatedOrganizationId = Organization.MerId,
                RelatedCampaignId = CampaignId,
                AcquireEmailStatus = Status,
                InsertDate = DateTime.Now
            });
            db.SaveChanges();
        }

        public IList<AcquireEmailExportModel> GetEntityList(int CampaignId)
        {
            var entityList = (from ae in db.AcquireEmails
                              where ae.RelatedCampaignId == CampaignId && ae.AcquireEmailStatus == AcquireEmailStatusEnum.VERIFIED
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