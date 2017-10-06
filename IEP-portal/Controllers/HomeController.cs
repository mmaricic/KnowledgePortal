using IEP_portal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEP_portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(Request.IsAuthenticated)
                using( var db = new DatabaseModel())
                {
                    var user = db.AspNetUsers.Find(User.Identity.GetUserId());
                    return View(user);
                }
               
            return View();
        }

    }
}