using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEP_portal.Models;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using MyNamespace.Controllers;

namespace IEP_portal.Controllers
{
   
    public class OrdersController : Controller
    {
        private DatabaseModel db = new DatabaseModel();

        // GET: Orders
        [CustomAuthorize(Roles = "Teacher, Student")]
        public ActionResult Index()
        {
            var user = User.Identity.GetUserId();
            var order = db.Order.Where(o => o.UserId == user).Include(o => o.AspNetUsers);
            var param = db.Parameters.FirstOrDefault();
            ViewBag.Silver = param.S;
            ViewBag.Gold = param.G;
            ViewBag.Platinum = param.P;

            return View(order.ToList());
        }

        // GET: Orders/Details/5
        [CustomAuthorize(Roles = "Teacher, Student")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public HttpStatusCodeResult Payment(String userid, String status)
        {
            Order order = db.Order.Find(Convert.ToInt32(userid));
            if(!order.Status.Equals("Processing..."))
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable); 
            order.Status = status;
           

            if (status == "success")
            {
                var user = db.AspNetUsers.Find(order.UserId);
                try
                {
                    sendMail(user.Email, "Your order of " + order.Tokens + " tokens from Interactive Knowledge Portal is successful.");
                }
                catch (Exception e) { }
                user.Tokens += order.Tokens;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
                order.Status = "cancelled";

            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return new HttpStatusCodeResult(200);
        }


        [AllowAnonymous]
        private void sendMail(String receiver, String body)
        {

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("myemail@gmail.com", "IEP Portal Team");
            mail.To.Add(receiver);
            mail.Subject = "Token Order - Interactive knowledge testing portal";
            mail.Body = body;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Port = 587;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.Credentials = new System.Net.NetworkCredential("myemail@gmail.com", "mypassword");
            SmtpServer.EnableSsl = true;
            
          
            SmtpServer.Send(mail);
        }

       [HttpPost]
       [ValidateHeaderAntiForgeryToken]
        [CustomAuthorize(Roles = "Teacher, Student")]
        public JsonResult NewOrder(int amount, int price)
        {
            var order = new Order();
            order.creationTime = DateTime.Now;
            order.Price = price;
            order.Status = "Processing...";
            order.Tokens = amount;
            order.UserId = User.Identity.GetUserId();
            db.Order.Add(order);
            db.SaveChanges();

            return Json(new { id = order.Id});
        }

        public ActionResult Failed(String userid, String status)
        {
            Payment(userid, status);
            return View();
        }

        public ActionResult Success(String userid, String status)
        {
            Payment(userid, status);
            return View();
        }
    }
}
