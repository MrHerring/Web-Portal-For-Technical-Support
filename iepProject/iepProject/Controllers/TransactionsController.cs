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
    public class TransactionsController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: Transactions
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                var userId = (int)Session["userId"];
                var transactions = db.Transactions.Include(t => t.User).Where(t => t.UserId == userId);
                return View(transactions.ToList());
            }
            else
            {
                return RedirectToAction("Home", "Index");
            }
            
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);

            if (Session["user"] != null)
            {
                if (transaction.UserId == (int)Session["userId"])
                {
                    if (transaction == null)
                    {
                        return HttpNotFound();
                    }
                    return View(transaction);
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

      
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
