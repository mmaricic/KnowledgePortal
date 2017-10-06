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
using MyNamespace.Controllers;
using Microsoft.AspNet.SignalR;
using IEP_portal.Hubs;
using Newtonsoft.Json;

namespace IEP_portal.Controllers
{
   
    public class ChannelsController : Controller
    {
        private DatabaseModel db = new DatabaseModel();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
   (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Channels
        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            var channel = db.Channel.Where(c => c.TeacherId == id).Include(c => c.AspNetUsers);
            foreach (var ch in channel)
                if (ch.timeLimit != null && ch.ClosedAt == null)
                    checkTime(ch);
            return View(channel.ToList());
        }

        // GET: Channels/Details/5
        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = db.Channel.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            if (channel.timeLimit != null && channel.ClosedAt == null)
                checkTime(channel);
            return View(channel);
        }

        // GET: Channels/Create
        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Create()
        {
            ViewBag.TeacherId = new SelectList(db.AspNetUsers, "Id", "Name");
            return View();
        }

        // POST: Channels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Create([Bind(Include = "Id,Title, Password, timeLimit")] Channel channel)
        {
            try
            {

                ModelState["TeacherId"].Errors.Clear();
                if (ModelState.IsValid)
                {
                    channel.TeacherId = User.Identity.GetUserId();
                    channel.StudentsNum = 0;
                    db.Channel.Add(channel);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    log.Info("Model State for creating Channel is not valid!");
            }catch(Exception e)
            {
                log.Error(e);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(channel);
        }

        // GET: Channels/Edit/5
        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = db.Channel.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeacherId = new SelectList(db.AspNetUsers, "Id", "Name", channel.TeacherId);
            return View(channel);
        }

        // POST: Channels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Edit([Bind(Include = "Id,Title,OpenedAt,Password,ClosedAt,StudentsNum,TeacherId")] Channel channel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(channel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
                log.Info("Model state for editing channel is not valid");

            ViewBag.TeacherId = new SelectList(db.AspNetUsers, "Id", "Name", channel.TeacherId);
            return View(channel);
        }

       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool checkTime(Channel channel)
        {
            if (channel.timeLimit == null || channel.OpenedAt == null)
                return true;
           
            DateTime closed = ((DateTime)(channel.OpenedAt)).AddMinutes((int)channel.timeLimit);
            if (DateTime.Compare(closed, DateTime.Now) <= 0)
            {
                channel.ClosedAt = closed;
                return false;
            }
            return true;
        }

        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Questions(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = db.Channel.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            var questions = db.Distributed_Question.Where(x => x.ChannelId == id).Include(x => x.Distributed_Answer);
            return View(questions.ToList());
           
        }

        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult QuestionAnswers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Distributed_Question question = db.Distributed_Question.Where(x => x.Id == id).Include(x => x.Distributed_Answer).FirstOrDefault();

            if (question == null)
            {
                return HttpNotFound();
            }
            var answers = db.Response.Where(x => x.QuestionId == id).Include(x => x.Subscribed.AspNetUsers).Include(x => x.Distributed_Answer);

            return View(new DisplayAnswersViewModel { question = question, answers = answers.ToList()});

        }

        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Distribute(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string user = User.Identity.GetUserId();
            var question = from s in db.Question
                                where (s.AuthorId == user) && (s.IsLocked) && (!(from o in db.Distributed_Question
                                                                                where o.ChannelId == id
                                                                                select o.ParentId).Contains(s.Id)
                             || (s.UpdatedAt >=
                                db.Distributed_Question.Where(c => c.ParentId == s.Id).Where(x => x.ChannelId == id)
                                .OrderByDescending(t => t.CreatedAt).Select(x => x.CreatedAt)
                                .FirstOrDefault()))
                           select s;
            ViewBag.channelId = id;
            return View(question.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Distribute(int id, int [] selected)
        {
            var questions = new Distributed_Question[selected.Length];
            var cnt = 0;
            try
            {
                foreach (int i in selected)
                {
                    Distributed_Question dq = new Distributed_Question();
                    dq.ChannelId = id;
                    dq.ParentId = i;
                    dq.CreatedAt = DateTime.Now;
                    Question question = db.Question.Find(i);
                    dq.Text = question.Text;
                    dq.Picture = question.Picture;
                    dq.Title = question.Title;
                    db.Distributed_Question.Add(dq);
                    foreach (var ans in question.Answer)
                    {
                        Distributed_Answer da = new Distributed_Answer();
                        da.IsCorrect = ans.IsCorrect;
                        da.Mark = ans.Mark;
                        da.QuestionId = dq.Id;
                        da.Text = ans.Text;
                        db.Distributed_Answer.Add(da);
                    }
                    db.SaveChanges();

                    questions[cnt++] = db.Distributed_Question.Include(q => q.Distributed_Answer).Include(q => q.Channel).Where(q => q.Id == dq.Id).First();
                }
                var context = GlobalHost.ConnectionManager.GetHubContext<QuestionHub>();
                context.Clients.Group("Channel " + id).newQuestions(JsonConvert.SerializeObject(questions, Formatting.Indented, new JsonSerializerSettings()
                { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore }));
            }catch(Exception e)
            {
                log.Error(e);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { result = "Redirect", url = Url.Action("Questions", "Channels", new { id = id }) });
        }

        [CustomAuthorize(Roles = "Student")]
        public ActionResult ActiveChannels()
        {
            List<ActiveChannelsViewModel> list = new List<ActiveChannelsViewModel>();

            try
            {
                var channels = db.Channel.Where(c => c.ClosedAt == null).Where(c => c.OpenedAt != null);
                var user = User.Identity.GetUserId();
                foreach (var ch in channels)
                    if (checkTime(ch))
                    {
                        var item = new ActiveChannelsViewModel();
                        item.channel = ch;
                        if (db.Subscribed.Where(x => x.Channelid == ch.Id).Where(x => x.StudentId == user).Any())
                        {
                            item.isSubscribed = true;
                            item.evaluation = db.Subscribed.Where(x => x.Channelid == ch.Id).Where(x => x.StudentId == user).Select(x => x.evaluation).FirstOrDefault();

                        }
                        else
                            item.isSubscribed = false;
                        list.Add(item);
                    }
                Session["closedChannel"] = null;
            }catch(Exception e)
            {
                log.Error(e);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(list);
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [CustomAuthorize(Roles = "Student")]
        public ActionResult Subscribe(int id, string password, bool evaluation)
        {
            var channel = db.Channel.Find(id);
            if (channel.ClosedAt == null && checkTime(channel))
            {
                if (channel.Password.Equals(password))
                {
                    var subcr = new Subscribed();
                    subcr.Channelid = id;
                    subcr.evaluation = evaluation;
                    subcr.StudentId = User.Identity.GetUserId();
                    subcr.Time = DateTime.Now;
                    db.Subscribed.Add(subcr);
                    db.SaveChanges();

                    channel.StudentsNum = channel.StudentsNum + 1;
                    db.Entry(channel).State = EntityState.Modified;
                    db.SaveChanges();

                   // Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new {state = "valid", result = "Redirect", url = Url.Action("ActiveChannels", "Channels") });
                }
               // Response.StatusCode = (int)HttpStatusCode.BadRequest;
                if(password.Equals("") || password == null)
                    return Json(new { state = "invalid", error = "Password is required" });
                return Json(new { state = "invalid", error = "Password is incorrect" });
            }
            Session["closedChannel"] = "Subscription failed because requested channel is closed";
           // Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { state = "closed", url = Url.Action("ActiveChannels", "Channels")});

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Student")]
        public ActionResult evaluationOn(int id)
        {
            var channel = db.Channel.Find(id);
            if (channel.ClosedAt == null && checkTime(channel)) {
                if (db.Parameters.FirstOrDefault().E == null)
                {
                    Session["closedChannel"] = "You can't turn on evaluation because Admin hasn't defined price for evaluation.";
                }
                else
                {
                    var user = User.Identity.GetUserId();
                    var sub = db.Subscribed.Where(s => s.Channelid == id).Where(s => s.StudentId == user).FirstOrDefault();
                    sub.evaluation = true;
                    db.Entry(sub).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["closedChannel"] = null;
                }
            }
            else
            {
                Session["closedChannel"] = "Change of Auto-Evaluation state failed because requested channel is closed";
            }

            return RedirectToAction("ActiveChannels");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Student")]
        public ActionResult evaluationOff(int id)
        {
            var channel = db.Channel.Find(id);
            if (channel.ClosedAt == null && checkTime(channel))
            {
                var user = User.Identity.GetUserId();
                var sub = db.Subscribed.Where(s => s.Channelid == id).Where(s => s.StudentId == user).FirstOrDefault();
                sub.evaluation = false;
                db.Entry(sub).State = EntityState.Modified;
                db.SaveChanges();
                Session["closedChannel"] = null;
            }
            else
            {
                Session["closedChannel"] = "Change of Auto-Evaluation state failed because requested channel is closed";
            }

            return RedirectToAction("ActiveChannels");
        }

        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Open(int id)
        {
            var channel = db.Channel.Find(id);
            if (channel.OpenedAt == null)
            {
                channel.OpenedAt = DateTime.Now;
                db.Entry(channel).State = EntityState.Modified;
                db.SaveChanges();
                var context = GlobalHost.ConnectionManager.GetHubContext<ChannelHub>();
                context.Clients.All.addChannel(JsonConvert.SerializeObject(channel, Formatting.Indented, new JsonSerializerSettings()
                { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore }));
            }

            return RedirectToAction("Details", new { id = id });
        }

        [CustomAuthorize(Roles = "Teacher")]
        public ActionResult Close(int id)
        {
            var channel = db.Channel.Find(id);
            if (channel.timeLimit == null && channel.OpenedAt != null)
                if (channel.ClosedAt == null)
                {
                    channel.ClosedAt = DateTime.Now;
                    db.Entry(channel).State = EntityState.Modified;
                    db.SaveChanges();
                    var context = GlobalHost.ConnectionManager.GetHubContext<ChannelHub>();
                    context.Clients.All.removeChannel(id);
                }

            return RedirectToAction("Details", new { id = id});
        }

        [CustomAuthorize(Roles = "Student")]
        public ActionResult AddChannel(Channel channel)
        {
            var item = new ActiveChannelsViewModel();
            item.channel = channel;
            item.isSubscribed = false;
            item.evaluation = null;


            return PartialView("ChannelDisplay", item);
        }

    }

}
