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

namespace MojCRM.Areas.Campaigns.Controllers
{
    public class CampaignsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Campaigns/Campaigns
        public ActionResult Index()
        {
            var campaigns = db.Campaigns.Include(c => c.RelatedCompany);
            return View(campaigns.ToList().OrderByDescending(c => c.InsertDate));
        }

        // GET: Campaigns/Campaigns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // GET: Campaigns/Campaigns/Create
        public ActionResult Create()
        {
            //ViewBag.RelatedCompanyId = new SelectList(db.Organizations.Where(o => o.SubjectBusinessUnit == ""), "MerId", "SubjectName");
            return View();
        }

        // POST: Campaigns/Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public ActionResult CreateOld([Bind(Include = "CampaignId,CampaignName,CampaignDescription,CampaignInitiatior,RelatedCompanyId,CampaignType,CampaignStatus,CampaignStartDate,CampaignPlannedEndDate,CampaignEndDate,InsertDate,UpdateDate")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RelatedCompanyId = new SelectList(db.Organizations, "MerId", "VAT", campaign.RelatedCompanyId);
            return View(campaign);
        }

        // POST: Campaigns/Campaigns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCampaign Model)
        {
            db.Campaigns.Add(new Campaign
            {
                CampaignName = Model.CampaignName,
                CampaignDescription = Model.CampaignDescription,
                CampaignInitiatior = User.Identity.Name,
                RelatedCompanyId = 111955,
                CampaignType = Model.CampaignType,
                CampaignStatus = Campaign.CampaignStatusEnum.START,
                CampaignStartDate = Model.CampaignStartDate,
                CampaignPlannedEndDate = Model.CampaignPlannedEndDate,
                InsertDate = DateTime.Now
            });
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Campaigns/Campaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            ViewBag.RelatedCompanyId = new SelectList(db.Organizations.Where(o => o.MerId == 111955), "MerId", "VAT", campaign.RelatedCompanyId);
            return View(campaign);
        }

        // POST: Campaigns/Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CampaignId,CampaignName,CampaignDescription,CampaignInitiatior,RelatedCompanyId,CampaignType,CampaignStatus,CampaignStartDate,CampaignPlannedEndDate,CampaignEndDate,InsertDate,UpdateDate")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaign).State = EntityState.Modified;
                db.Entry(campaign).Entity.UpdateDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RelatedCompanyId = new SelectList(db.Organizations, "MerId", "VAT", campaign.RelatedCompanyId);
            return View(campaign);
        }

        // GET: Campaigns/Campaigns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // POST: Campaigns/Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            db.Campaigns.Remove(campaign);
            db.SaveChanges();
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
    }
}
