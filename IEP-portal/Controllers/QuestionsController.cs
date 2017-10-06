using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IEP_portal.Models;
using Microsoft.AspNet.Identity;
using MyNamespace.Controllers;


namespace IEP_portal.Controllers
{
   [CustomAuthorize(Roles ="Teacher")]
    public class QuestionsController : Controller
    {
        private DatabaseModel db = new DatabaseModel();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            var question = db.Question.Where(x => x.AuthorId == id).Include(q => q.AspNetUsers).OrderByDescending(x=>x.CreatedAt);
            Session["unlockErrors"] = null;
            Session["unlockSuccess"] = null;
            return View(question.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                log.Info("Bad request for question details");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var question = db.Question.Where(x => x.Id == id).Include(x => x.Answer).FirstOrDefault<Question>();
            if (question == null)
            {
                return HttpNotFound();
            }
            if (question.AuthorId != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
           
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create()
        {
            
            ViewBag.K = db.Parameters.Select(p => p.K).First();
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Create(CreateQuestionViewModel data, int correct)
        { //prilikom greske vratiti na istu stranicu i prikazati sve greske!
            try
            {
                data.question.AuthorId = User.Identity.GetUserId();
                if (data.question.UploadImage == null && (data.question.Text == null || data.question.Text.Equals("")))
               
                {
                    ModelState["question.Text"].Errors.Add("Text or Picture field is required");
                    if (ModelState.ContainsKey("question.UploadImage"))
                        ModelState["question.UploadImage"].Errors.Add("Text or Picture field is required");
                    else
                    ModelState.AddModelError("question.UploadImage", "Text or Picture field is required");
                    
                  
                }

                ModelState["question.IsLocked"].Errors.Clear();
                    ModelState["question.AuthorId"].Errors.Clear();
                    if (ModelState.IsValid)
                    {
                    if (data.question.UploadImage != null)
                    {
                        data.question.Picture = new byte[data.question.UploadImage.ContentLength];
                        data.question.UploadImage.InputStream.Read(data.question.Picture, 0, data.question.Picture.Length);
                    }
                        data.question.CreatedAt = DateTime.Now;
                        db.Question.Add(data.question);
                        foreach (Answer ans in data.answers)
                        {
                            ans.QuestionId = data.question.Id;
                            if (correct == ans.Number)
                                ans.IsCorrect = true;
                            else
                                ans.IsCorrect = false;
                            db.Answer.Add(ans);
                        }

                        db.SaveChanges();

                       // Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(new {status= true, result = "Redirect", url = Url.Action("Index", "Questions") });
                    }

       


               

                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new
                { status = false, 
                        errors = ModelState.Keys.Where(k => ModelState[k].Errors.Count > 0)
                        .Select(k => new { Name = k, error = ModelState[k].Errors[0].ErrorMessage })
                    });

            }
            catch (Exception e)
            {
                log.Error(e);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Question.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            if (question.IsLocked)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

                if (question.AuthorId != User.Identity.GetUserId())
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
           
            Session["Locked"] = question.IsLocked;
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AuthorId,Title,Text,IsLocked,Answer,Picture,CreatedAt,LockedAt,NewImage")] Question question, int CorrectAnswer)
        {
            try {
                if (question.Text == null && question.Picture == null && question.NewImage == null)
                {
                    if (ModelState.ContainsKey("Text"))
                        ModelState["Text"].Errors.Add("Text or Picture is required");
                    else
                        ModelState.AddModelError("Text", "Text or Picture is required");
                    if (ModelState.ContainsKey("NewImage"))
                        ModelState["NewImage"].Errors.Add("Text or Picture is required");
                    else
                        ModelState.AddModelError("NewImage", "Text or Picture is required");
                }
                if (ModelState.IsValid)
                {
                    if ((bool)Session["Locked"] == false && question.IsLocked == true)
                        question.LockedAt = DateTime.Now;
                    question.UpdatedAt = DateTime.Now;
                    if (question.NewImage != null)
                    {
                        question.Picture = new byte[question.NewImage.ContentLength];
                        question.NewImage.InputStream.Read(question.Picture, 0, question.Picture.Length);
                    }
                    db.Entry(question).State = EntityState.Modified;
                    foreach (var child in question.Answer)
                    {
                        if (child.Number == CorrectAnswer)
                            child.IsCorrect = true;
                        else
                            child.IsCorrect = false;
                        db.Entry(child).State = EntityState.Modified;
                       
                    }
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = question.Id });
                }
                else
                    log.Info("Editing Question failed - Model state is not valid ");
            }
            catch (Exception e)
            {
                log.Error(e);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.AuthorId = new SelectList(db.AspNetUsers, "Id", "Name", question.AuthorId);
            return View(question);
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Unlock(int? id)
        {

            Session["unlockErrors"] = null;
            Session["unlockSuccess"] = null;
            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                string authorId = User.Identity.GetUserId();
                var param = db.Parameters.First<Parameters>();
                Question question = db.Question.Find(id);
              
                if (param.M == null)
                    Session["unlockErrors"] = "Price for unlocking is still undefined. You cannot unlock your question.";
                else
                {
                    AspNetUsers user = db.AspNetUsers.Find(authorId);
                    if (param.M <= user.Tokens)
                    {
                        user.Tokens = user.Tokens - (int)param.M;
                        question.IsLocked = false;
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Session["unlockSuccess"] = "The question is unlocked.";
                    }
                    else
                        Session["unlockErrors"] = "You don't have enough tokens.";
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Details", new { id = id });

        }

       
    }
}
