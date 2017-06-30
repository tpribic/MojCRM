﻿using MojCRM.Models;
using MojCRM.Helpers;
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

            return View(Organizations.ToList().OrderBy(o => o.MerId));
        }

        // POST: Organizations/GetOrganizations
        public void GetOrganizations(string Name)
        {
            var MerUser = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserUsername).First();
            var MerPass = (from u in db.Users
                           where u.UserName == Name
                           select u.MerUserPassword).First();

            var ReferencedId = (from o in db.Organizations
                               orderby o.MerId descending
                               select o.MerId).First();
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
                    MerApiGetSubjekt Request = new MerApiGetSubjekt();

                    Request.Id = MerUser.ToString();
                    Request.Pass = MerPass.ToString();
                    Request.Oib = "99999999927";
                    Request.PJ = "";
                    Request.SoftwareId = "MojCRM-001";
                    Request.SubjektPJ = ReferencedId.ToString();

                    string MerRequest = JsonConvert.SerializeObject(Request);

                    using (var Mer = new WebClient() { Encoding = Encoding.UTF8 })
                    {
                        Mer.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        Mer.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                        var _Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                        _Response = _Response.Replace("[", "").Replace("]", "");
                        MerGetSubjektDataResponse Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse>(_Response);
                        db.Organizations.Add(new Organizations
                        {
                            MerId = Result.Id,
                            SubjectName = Result.Naziv,
                            SubjectBusinessUnit = Result.PoslovnaJedinica,
                            VAT = Result.Oib
                        });
                        db.SaveChanges();
                        Result = Response;
                    }
                    ReferencedId++;
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
                    User = Name,
                    InsertDate = DateTime.Now
                });
                db.SaveChanges();
            }
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