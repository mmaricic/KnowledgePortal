using IEP_portal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace IEP_portal.Controllers
{
    [CustomAuthorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private DatabaseModel db = new DatabaseModel();


        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult UnansweredQuestions()
        {
            var user = User.Identity.GetUserId();

            var questions = from q in db.Distributed_Question
                            from s in db.Subscribed
                            from c in db.Channel
                            where s.Channelid == q.ChannelId && q.ChannelId == c.Id && c.ClosedAt == null && s.StudentId == user && !((from r in db.Response
                                                                                                                                       where r.StudentId == user
                                                                                                                                       select r.QuestionId).Contains(q.Id))
                            select q;
            var count = db.Subscribed.Where(x => x.StudentId == user).Count();

            ViewBag.Channels = count;
            return View(questions.ToList());
        }


        public ActionResult AnswerQuestion(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var question = db.Distributed_Question.Find(id);

            return View(question);
        }

        [HttpPost]
        public ActionResult NewQuestion(Distributed_Question question)
        {
            return PartialView("NewQuestion", question);
        }

   
            

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAnswer(int id, int channel, int chosen)
        {
            String message = "";
            var color = "black";
            try
            {
                var ch = db.Channel.Find(channel);
                if (ch.timeLimit != null && ch.ClosedAt != null)
                {
                    DateTime closed = ((DateTime)(ch.OpenedAt)).AddMinutes((int)ch.timeLimit);
                    if (DateTime.Compare(closed, DateTime.Now) <= 0)
                    {
                        ch.ClosedAt = closed;
                    }
                }
               
                if (ch.ClosedAt != null)
                    message = "Your answer is not saved because channel is closed. We are sorry.";
                else {
                    var response = new Response();
                  
                    response.AnswerId = chosen;
                    response.QuestionId = id;
                    response.SentAt = DateTime.Now;
                    response.StudentId = User.Identity.GetUserId();
                    response.Channelid = channel;
                    

                    var user = db.AspNetUsers.Find(response.StudentId);
                    var sub = db.Subscribed.Where(x => x.Channelid == channel).Where(x => x.StudentId == response.StudentId).FirstOrDefault();
                    if (sub == null || user == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    if (sub.evaluation)
                    {
                        var param = db.Parameters.FirstOrDefault();
                        if (user.Tokens >= param.E)
                        {
                            user.Tokens = user.Tokens - (int)param.E;
                            if (db.Distributed_Answer.Find(chosen).IsCorrect)
                            {
                                message = "correct";
                                response.Correct = true;
                                color = "green";
                            }
                            else
                            {
                                message = "incorrect";
                                response.Correct = false;
                                color = "red";
                            }
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                           
                        }
                        else
                            message = "noTokens";
                        

                    }
                    else
                        message = "evalOff";

                    db.Response.Add(response);

                }
            }catch(Exception e)
            {
                log.Error(e);
            }
           
            db.SaveChanges();
            Session["submitMessage"] = message;
            Session["submitColor"] = color;
            return RedirectToAction("AnswerSubmission");
        }

        public ActionResult AnswerSubmission()
        {
           
            return View();
        }

        public ActionResult MyAnswers()
        {
            var user = User.Identity.GetUserId();
            var responses = db.Response.Where(x => x.StudentId == user);

            return View(responses.ToList());
        }

        public ActionResult MyAnswerDetails(string sid, int qid)
        {
            var response = db.Response.Where(x => x.StudentId == sid).Where(x => x.QuestionId == qid).FirstOrDefault();
            if (response == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(response);
        } 
    }
}

