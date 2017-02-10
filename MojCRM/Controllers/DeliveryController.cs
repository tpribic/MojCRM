﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MojCRM.Models;
using ActiveUp.Net.Mail;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MojCRM.Helpers;
using PagedList;

namespace MojCRM.Controllers
{
    public class DeliveryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Delivery
        public async Task<ActionResult> Index(string SortOrder, string Sender, string Receiver, string InvoiceNumber)
        {
            ViewBag.InsertDateParm = String.IsNullOrEmpty(SortOrder) ? "InsertDate" : "";
            ViewBag.SentDateParm = SortOrder == "SentDate" ? "SentDateDesc" : "SentDate";
            ViewBag.DocumentTypeParm = SortOrder == "DocType" ? "DocTypeDesc" : "DocType";

            var Results = from t in db.DeliveryTicketModels
                          where t.DocumentStatus == 30
                          select t;

            ViewBag.OpenTickets = Results.Count();

            if (!String.IsNullOrEmpty(Sender))
            {
                Results = Results.Where(t => t.Sender.SubjectName.Contains(Sender));
            }

            if (!String.IsNullOrEmpty(Receiver))
            {
                Results = Results.Where(t => t.Receiver.SubjectName.Contains(Receiver));
            }

            if (!String.IsNullOrEmpty(InvoiceNumber))
            {
                Results = Results.Where(t => t.InvoiceNumber.Contains(InvoiceNumber));
            }

            switch (SortOrder)
            {
                case "InsertDate":
                    Results = Results.OrderBy(d => d.InsertDate);
                    break;
                case "SentDate":
                    Results = Results.OrderBy(d => d.SentDate);
                    break;
                case "SentDateDesc":
                    Results = Results.OrderByDescending(d => d.SentDate);
                    break;
                case "DocType":
                    Results = Results.OrderBy(d => d.MerDocumentTypeId);
                    break;
                case "DocTypeDesc":
                    Results = Results.OrderByDescending(d => d.MerDocumentTypeId);
                    break;
                default:
                    Results = Results.OrderByDescending(d => d.InsertDate);
                    break;
            }

            return View(await Results.Take(20).ToListAsync());
        }

        // GET : Delivery/CreateTickets
        public ActionResult CreateTickets()
        {
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
                                    });
                                    db.SaveChanges();
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

        //GET: Delivery/UpdateAllStatuses
        public ActionResult UpdateAllStatuses()
        {
            var OpenTickets = from t in db.DeliveryTicketModels
                              where t.DocumentStatus == 30
                              select t.MerLink;

            var MerLinks = OpenTickets.ToList();

            foreach (var Link in MerLinks)
            {
                    MerDeliveryJsonResponse Result = ParseJson(Link);

                    var TicketForUpdate = from t in db.DeliveryTicketModels
                                          where t.MerLink == Link
                                          select t;
                    foreach (Delivery t in TicketForUpdate)
                    {
                        t.DocumentStatus = Result.Status;
                        t.UpdateDate = DateTime.Now;
                    }
                        db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Delivery/Details/5
        public async Task<ActionResult> Details(int? id)
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

        // GET: Delivery/Create
        public ActionResult Create()
        {
            return View();
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
    }
}
