using MojCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}