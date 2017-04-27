using MojCRM.Models;
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
        public ActionResult Index()
        {
            return View();
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
            ReferencedId++;
            try
            {
                while (ReferencedId > 0)
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
                        var Response = Mer.UploadString(new Uri(@"https://www.moj-eracun.hr/apis/v21/getSubjektData").ToString(), "POST", MerRequest);
                        //Response = Response.Replace("[", "").Replace("]", "");
                        MerGetSubjektDataResponse [] Result = JsonConvert.DeserializeObject<MerGetSubjektDataResponse[]>(Response);
                        db.Organizations.Add(new Organizations
                        {
                            MerId = Result[0].Id,
                            SubjectName = Result[0].Naziv,
                            SubjectBusinessUnit = Result[0].PoslovnaJedinica,
                            VAT = Result[0].Oib
                        });
                        db.SaveChanges();
                    }
                    ReferencedId++;
                }
            }
            catch (Exception)
            {
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