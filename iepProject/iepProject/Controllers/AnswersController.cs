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

namespace iepProject.Controllers
{
    public class AnswersController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Answers
        public ActionResult Index()
        {
            var answers = db.Answers.Include(a => a.Author).Include(a => a.Question);
            return View(answers.ToList());
        }

        // GET: Answers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // GET: Answers/Create
        public ActionResult Create(int? id)
        {

            if (Session["user"] != null && id != null)
            {
                ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title");

                ViewData["questionId"] = id;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

           
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,AuthorId,QuestionId,TimeCreated,PositiveRatingCnt,NegativeRatingCnt")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                answer.TimeCreated = DateTime.UtcNow;

                db.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("../Questions/Details/", new { id = answer.QuestionId});
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", answer.AuthorId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", answer.QuestionId);
            return View(answer);
        }

        // GET: Answers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", answer.AuthorId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", answer.QuestionId);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,AuthorId,QuestionId,TimeCreated,PositiveRatingCnt,NegativeRatingCnt")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", answer.AuthorId);
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", answer.QuestionId);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
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


        public ActionResult AddPositiveGrade(bool isPositive, int answerId, int authorId)
        {

            Grade grade = db.Grades.SingleOrDefault(g => g.AuthorId == authorId && g.AnswerId == answerId);

            Answer answer = db.Answers.SingleOrDefault(a => a.Id == answerId);

            answer.PositiveRatingCnt++;

            if (grade == null)
            {
                grade = new Grade();

                grade.IsPositive = isPositive;
                grade.AnswerId = answerId;
                grade.AuthorId = authorId;

                db.Grades.Add(grade);
                db.SaveChanges();
            }
            else
            {

                answer.NegativeRatingCnt--;

                grade.IsPositive = true;
                db.SaveChanges();
            }

            

            return RedirectToAction("../Questions/Details", new { id = answer.QuestionId });
        }

        public ActionResult AddNegativeGrade(bool isPositive, int answerId, int authorId)
        {

            Grade grade = db.Grades.SingleOrDefault(g => g.AuthorId == authorId && g.AnswerId == answerId);

            Answer answer = db.Answers.SingleOrDefault(a => a.Id == answerId);

            answer.NegativeRatingCnt++;

            if (grade == null)
            {
                grade = new Grade();

                grade.IsPositive = isPositive;
                grade.AnswerId = answerId;
                grade.AuthorId = authorId;

                db.Grades.Add(grade);
                db.SaveChanges();
            }
            else
            {

                answer.PositiveRatingCnt--;

                grade.IsPositive = false;
                db.SaveChanges();
            }



            return RedirectToAction("../Questions/Details", new { id = answer.QuestionId });
        }



    }
}
