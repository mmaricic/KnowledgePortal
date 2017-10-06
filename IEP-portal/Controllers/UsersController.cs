using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEP_portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IEP_portal.Controllers
{
    public class UsersController : Controller
    {
        private DatabaseModel db = new DatabaseModel();

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        // GET: Users/Details/5
        [CustomAuthorize(Roles = "Student, Teacher")]
        public ActionResult Details()
        {
            var param = db.Parameters.FirstOrDefault();
            ViewBag.Silver = param.S;
            ViewBag.Gold = param.G;
            ViewBag.Platinum = param.P;

            AspNetUsers aspNetUser = db.AspNetUsers.Find(User.Identity.GetUserId());
            return View(aspNetUser);
        }

       
        // GET: Users/Edit/5
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Activate(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            if(aspNetUsers.isActive)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            ApplicationDbContext context = new ApplicationDbContext();
            ViewBag.Name = new SelectList(new List < Object > {
            new {value = "Teacher", text= "Teacher"},
            new {value = "Student", text = "Student"}
       }, "value", "text", " ");
            return View(aspNetUsers);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Activate(EditViewModel model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUser user = context.Users.Where(x => x.Id == model.Id).First<ApplicationUser>();
            if (ModelState.IsValid)
            {
                user.isActive = model.isActive;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                UserManager.AddToRole(user.Id, model.Role);
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
    }
}
