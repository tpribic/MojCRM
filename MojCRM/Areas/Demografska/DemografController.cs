using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojCRM.Areas.Demografska
{
    public class DemografController : Controller
    {
        // GET: Demografska/Demograf
        public ActionResult Index()
        {
            return View();
        }

        // GET: Demografska/Demograf/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Demografska/Demograf/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Demografska/Demograf/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Demografska/Demograf/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Demografska/Demograf/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Demografska/Demograf/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Demografska/Demograf/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
