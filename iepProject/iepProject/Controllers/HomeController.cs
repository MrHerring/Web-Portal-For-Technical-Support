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
using iepProject.PayPal;


namespace iepProject.Controllers
{
    public class HomeController : Controller
    {

        ApplicationContext db = new ApplicationContext();

        public ActionResult Index(int categoryId = -1)
        {
            List<Question> questions = null;

            if (categoryId == -1)
            {
                questions = db.Questions.Include(q => q.Author).Include(q => q.Category).ToList();
            }
            else
            {
                questions = db.Questions.Include(q => q.Author).Include(q => q.Category).Where(q => q.CategoryId == categoryId).ToList();
            }

            ViewBag.Categories = db.Categories.ToList();

            return View(questions);
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Details(int? id)
        {
            return RedirectToAction("../Questions/Details", new { id = id });
        }

        public ActionResult Edit(int? id)
        {
            return RedirectToAction("../Questions/Edit", new { id = id });
        }

        public ActionResult Delete(int? id)
        {
            return RedirectToAction("../Questions/Delete", new { id = id });
        }

        public ActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginSubmit(String email, String password)
        {
            if (Session["user"] == null)
            {
                var unicodeCoder = new System.Text.UnicodeEncoding();
                var byteEncodedPassword = unicodeCoder.GetBytes(password);
                var byteHashPassword = new System.Security.Cryptography.SHA256Managed().ComputeHash(byteEncodedPassword);
                password = Convert.ToBase64String(byteHashPassword);


                User user = db.Users.SingleOrDefault(s => s.Email == email && s.Password == password && s.Status == true);

                if (user != null)
                {
                    Session["user"] = user;
                    Session["role"] = user.Role;
                    Session["email"] = user.Email;
                    Session["userId"] = user.Id;
                    Session["username"] = user.FirstName;
                }
                else
                {
                    return RedirectToAction("Login");
                }

            }

            return RedirectToAction("Index");
        }


        public ActionResult Logout()
        {
            Session["user"] = null;
            Session["role"] = null;

            return RedirectToAction("Index");
        }

        public ActionResult MyQuestions()
        {
            int userId = (int)Session["userId"];

            var questions = db.Questions.Include(q => q.Author).Include(q => q.Category).Where(q => q.AuthorId == userId);

            return View(questions.ToList());
        }


        public ActionResult UnlockQuestion(int? id)
        {
            var question = db.Questions.SingleOrDefault(q => q.Id == id);

            question.IsLocked = false;

            db.Entry(question).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult LockQuestion(int? id)
        {
            var question = db.Questions.SingleOrDefault(q => q.Id == id);

            question.IsLocked = true;

            db.Entry(question).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }





        //----- PayPal functions


        [HttpPost]
        public ActionResult PaypalTransactionComplete(String orderID)
        {
            if (Session["user"] != null)
            {
                int userId = (int)Session["userId"];
                GetOrderClass.GetOrder(orderID, userId);
                return RedirectToAction("Details/" + userId, "Users");
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SystemDetails()
        {

            if (Session["user"] == null || (Session["user"] != null && ((int)Session["role"]) != 0))
            {
                return RedirectToAction("Index");
            }

            ViewBag.SilverPrice = GetOrderClass.SilverPrice;
            ViewBag.SilverTokens = GetOrderClass.Silver;

            ViewBag.GoldPrice = GetOrderClass.GoldPrice;
            ViewBag.GoldTokens = GetOrderClass.Gold;

            ViewBag.PlatinumPrice = GetOrderClass.PlatinumPrice;
            ViewBag.PlatinumTokens = GetOrderClass.Platinum;

            ViewBag.ChatRoomPrice = ChatRoomsController.priceOfNewChatRoom;

            return View();
        }



        [HttpPost]
        public ActionResult SetSilverPrice(String NewPrice)
        {
            
            float result;
            bool test = float.TryParse(NewPrice, out result);
            if (test)
                GetOrderClass.SilverPrice = NewPrice;

            return RedirectToAction("SystemDetails");
        }

        [HttpPost]
        public ActionResult SetSilverToken(String NewCount)
        {

           
            int count = -1;
            bool test = Int32.TryParse(NewCount, out count);

            if (test)
                GetOrderClass.Silver = count;

            return RedirectToAction("SystemDetails");
        }

        [HttpPost]
        public ActionResult SetGoldPrice(String NewPrice)
        {

            
            float result;
            bool test = float.TryParse(NewPrice, out result);
            if (test)
                GetOrderClass.GoldPrice = NewPrice;

            return RedirectToAction("SystemDetails");
        }

        [HttpPost]
        public ActionResult SetGoldToken(String NewCount)
        {
            
            int count = -1;
            bool test = Int32.TryParse(NewCount, out count);

            if (test)
                GetOrderClass.Gold = count;

            return RedirectToAction("SystemDetails");
        }


        [HttpPost]
        public ActionResult SetPlatinumPrice(String NewPrice)
        {

           

            float result;
            bool test = float.TryParse(NewPrice, out result);
            if (test)
                GetOrderClass.PlatinumPrice = NewPrice;

            return RedirectToAction("SystemDetails");
        }

        [HttpPost]
        public ActionResult SetPlatinumToken(String NewCount)
        {

            

            int count = -1;
            bool test = Int32.TryParse(NewCount, out count);

            if (test)
                GetOrderClass.Platinum = count;

            return RedirectToAction("SystemDetails");
        }

        [HttpPost]
        public ActionResult SetChatRoomPrice(String NewPrice)
        {

          

            int count = -1;
            bool test = Int32.TryParse(NewPrice, out count);

            if (test)
                ChatRoomsController.priceOfNewChatRoom = count;

            return RedirectToAction("SystemDetails");
        }

        [HttpGet]
        public ActionResult Search(String SearchCriteria)
        {
            if (!String.IsNullOrEmpty(SearchCriteria))
            {
                var quesions = db.Questions.Where(q => q.Title.Contains(SearchCriteria));

                return View(quesions.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        public ActionResult Total()
        {
            ViewBag.user = db.Users.Where(u => u.Role == 1).Count();
            ViewBag.admin = db.Users.Where(u => u.Role == 0).Count();
            ViewBag.tech = db.Users.Where(u => u.Role == 2).Count();

            var list = db.Transactions.ToList();

            float sum = 0;

            foreach(var trans in list)
            {
                sum += trans.Paid;
            }

            ViewBag.sum = sum;

            return View();
        }



    }
}