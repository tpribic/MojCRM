using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Areas.Sales.Controllers
{
    public class OpportunitiesController : Controller
    {
        // GET: Sales/Opportunities
        public ActionResult Index()
        {
            return View();
        }
    }
}