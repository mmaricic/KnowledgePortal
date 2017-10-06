using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEP_portal.Models;

namespace IEP_portal.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ParametersController : Controller
    {
        private DatabaseModel db = new DatabaseModel();


        // GET: Parameters/Details
        public ActionResult Index()
        {
            Parameters parameters = db.Parameters.FirstOrDefault<Parameters>();
            return View(parameters);
        }

  
        // GET: Parameters/Edit
        public ActionResult Edit()
        {
            Parameters parameters = db.Parameters.FirstOrDefault<Parameters>();
            return View(parameters);
        }

        // POST: Parameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,K,M,E,S,G,P")] Parameters parameters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(parameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(parameters);
        }

    }
}
