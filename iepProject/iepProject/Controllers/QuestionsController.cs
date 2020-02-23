using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iepProject;
using iepProject.Models;
using System.IO;



namespace iepProject.Controllers
{
    public class QuestionsController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }

            var answers = db.Answers.Where(a => a.QuestionId == id).ToList();

            

            if (Session["sortCrit"] != null)
            {
                int sortCrit = (int)Session["sortCrit"];


                switch(sortCrit)
                {
                    case 1:
                        {
                            answers.Sort((x, y) => x.PositiveRatingCnt.CompareTo(y.PositiveRatingCnt));
                            answers.Reverse();
                            break;
                        }
                    case 2:
                        {
                            answers.Sort((x, y) => x.NegativeRatingCnt.CompareTo(y.NegativeRatingCnt));
                            answers.Reverse();
                            break;
                        }
                    case 3:
                        {
                            answers.Sort((x, y) => x.TimeCreated.CompareTo(y.TimeCreated));
                            break;
                        }
                }
                
            }

            

            Dictionary<int, int> pos_answerId_gradeCnt = new Dictionary<int, int>();
            Dictionary<int, int> neg_answerId_gradeCnt = new Dictionary<int, int>();

            //all answers for question
            foreach (var answer in answers)
            {
                if (!pos_answerId_gradeCnt.ContainsKey(answer.Id))
                {
                    pos_answerId_gradeCnt.Add(answer.Id, 0);
                    neg_answerId_gradeCnt.Add(answer.Id, 0);
                }

                var amountOfPositiveGradesForAnswer = db.Grades.Where(g => g.AnswerId == answer.Id && g.IsPositive == true).Count();
                var amountOfNegativeGradesForAnswer = db.Grades.Where(g => g.AnswerId == answer.Id && g.IsPositive == false).Count();

                pos_answerId_gradeCnt[answer.Id] += amountOfPositiveGradesForAnswer;
                neg_answerId_gradeCnt[answer.Id] += amountOfNegativeGradesForAnswer;
            }

            ViewBag.posDictionary = pos_answerId_gradeCnt;
            ViewBag.negDictionary = neg_answerId_gradeCnt;


            if (Session["user"] != null)
            {
                var userId = (int)Session["userId"];

                var positiveAnswers = db.Grades.Where(g => g.IsPositive == true && g.AuthorId == userId && g.Answer.QuestionId == id).ToList();
                var negativeAnswers = db.Grades.Where(g => g.IsPositive == false && g.AuthorId == userId && g.Answer.QuestionId == id).ToList();

                var postiveAnswerList = new List<int>();
                var negativeAnswerList = new List<int>();


                foreach (var answer in positiveAnswers)
                {
                    postiveAnswerList.Add(answer.AnswerId);
                }

                foreach (var answer in negativeAnswers)
                {
                    negativeAnswerList.Add(answer.AnswerId);
                }


                ViewBag.postiveAnswer = postiveAnswerList;
                ViewBag.negatveAnswer = negativeAnswerList;
            }

            ViewBag.answersList = answers;

            Dictionary<int, List<Reply>> answerId_reply = new Dictionary<int, List<Reply>>();

            foreach (var answer in answers)
            {
                var reply = db.Replies.Where(r => r.AnswerId == answer.Id).ToList();
                answerId_reply.Add(answer.Id, reply);
            }

            ViewBag.replyMap = answerId_reply;


            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create()
        {
            if (Session["user"] != null)
            {
                ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Text,ImagePath,IsLocked,AuthorId,CategoryId")] Question question)
        {
           
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Question question)
        {
            if (Session["user"] != null)
            {
                question.AuthorId = (int)Session["userId"];

                String filename = Path.GetFileNameWithoutExtension(question.ImageFile.FileName);
                String extension = Path.GetExtension(question.ImageFile.FileName);
                String lowerCaseExtension = extension.ToLower();

                if (lowerCaseExtension == ".jpg" || lowerCaseExtension == ".jpeg"
                           || lowerCaseExtension == ".png" || lowerCaseExtension == ".bmp")
                {

                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    question.ImagePath = "~/Content/" + filename;

                    filename = Path.Combine(Server.MapPath("~/Content/"), filename);
                    question.ImageFile.SaveAs(filename);
                }

                question.IsLocked = false;
                question.AuthorId = (int)Session["userId"];

                db.Questions.Add(question);

                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
               
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Question question = db.Questions.Find(id);

            if (Session["user"] != null)
            {
                if (((int)Session["userId"] == question.AuthorId))
                {
                    if (question == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", question.AuthorId);
                    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", question.CategoryId);
                    return View(question);
                }

                if ((int)Session["role"] == 0)
                {
                    if (question == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", question.AuthorId);
                    ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", question.CategoryId);
                    return View(question);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Text,Picture,IsLocked,AuthorId,CategoryId")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", question.AuthorId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", question.CategoryId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);

            if (Session["user"] != null)
            {
                if (((int)Session["userId"] == question.AuthorId))
                {
                    if (question == null)
                    {
                        return HttpNotFound();
                    }
                    return View(question);
                }

                if ((int)Session["role"] == 0) 
                {
                    if (question == null)
                    {
                        return HttpNotFound();
                    }
                    return View(question);
                }

                return RedirectToAction("Index", "Home");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Replies/Create
        public ActionResult CreateReply(int? id)
        {
            ViewBag.answerId = id;
            return View();
        }

        // POST: Replies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReply([Bind(Include = "Id,Text,AuthorId,AnswerId,TimeCreated")] Reply reply)
        {
            if (ModelState.IsValid)
            {
                reply.TimeCreated = DateTime.UtcNow;
                db.Replies.Add(reply);
                db.SaveChanges();

                var answer = db.Answers.SingleOrDefault(a => a.Id == reply.AnswerId);

                return RedirectToAction("Details", new { id = answer.QuestionId });
            }

            ViewBag.AnswerId = new SelectList(db.Answers, "Id", "Text", reply.AnswerId);
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", reply.AuthorId);


            return RedirectToAction("CreateReply", new { id = reply.Answer });
        }

        public ActionResult SortAnswers(int? sortCrit, int? id)
        {
            Session["sortCrit"] = sortCrit;

            return RedirectToAction("Details", new { id = id});
        }


    }
}
