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
    public class ChatRoomsController : Controller
    {

        public static int priceOfNewChatRoom = 5;

        private ApplicationContext db = new ApplicationContext();

        public ActionResult MyChatRooms()
        {
            Session["myChatRooms"] = true;

            return RedirectToAction("Index");
        }

        // GET: ChatRooms
        public ActionResult Index()
        {

            if(Session["user"] != null)
            {
                List<ChatRoom> chatRooms;

                int userId = (int)Session["userId"];

                if (Session["myChatRooms"] != null)
                {
                    chatRooms = db.ChatRooms.Include(c => c.Creator).Where(c => c.CreatorID == userId).ToList();
                    Session["myChatRooms"] = null;
                }
                else
                {
                    chatRooms = db.ChatRooms.Include(c => c.Creator).ToList();
                }

                Dictionary<Guid, int> chatRoomId_cntInChatRoom = new Dictionary<Guid, int>();

                foreach (var chatRoom in chatRooms)
                {
                    int cnt = db.InChatRooms.Where(c => c.ChatRoomId == chatRoom.Id).Count();
                    cnt = cnt - 1;
                    chatRoomId_cntInChatRoom.Add(chatRoom.Id, cnt);
                }

                ViewBag.techSupportCnt = chatRoomId_cntInChatRoom;


                List<InChatRoom> inChatRooms = db.InChatRooms.Where(c => c.UserId == userId).ToList();
                List<Guid> inChatRoomsGuid = new List<Guid>();

                foreach (var element in inChatRooms)
                {
                    inChatRoomsGuid.Add(element.ChatRoomId);
                }


                ViewBag.inChatRooms = inChatRoomsGuid;

                return View(chatRooms);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

           
        }

        public ActionResult Details(Guid? id)
        {
            int userId = (int)Session["userId"];

            var inChatRooms = db.InChatRooms.SingleOrDefault(c => c.ChatRoomId == id && c.UserId == userId);

            if (inChatRooms == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.chatMessagesList = db.Messages.Where(m => m.ChatRoomId == id).ToList();
            ChatRoom chatRoom = db.ChatRooms.Find(id);

            return View(chatRoom);
        }

        // GET: ChatRooms/Create
        public ActionResult Create()
        {
            if (Session["user"] != null && (int)Session["role"] == 1)
            {
                ViewBag.CreatorID = new SelectList(db.Users, "Id", "FirstName");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ChatRooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CreatorID,TimeCreated")] ChatRoom chatRoom)
        {
            User myUser = db.Users.SingleOrDefault(u => u.Id == chatRoom.CreatorID);

            if (myUser.TokenAmount >= priceOfNewChatRoom)
            {

                if (ModelState.IsValid)
                {
                    myUser.TokenAmount -= priceOfNewChatRoom;

                    chatRoom.Id = Guid.NewGuid();
                    chatRoom.TimeCreated = DateTime.UtcNow;
                   
                    //--- Adding user in chatRoom in Db ---
                    InChatRoom inChatRoom = new InChatRoom();

                    inChatRoom.ChatRoomId = chatRoom.Id;
                    inChatRoom.UserId = chatRoom.CreatorID;

                    db.ChatRooms.Add(chatRoom);
                    db.InChatRooms.Add(inChatRoom);

                    db.Entry(myUser).State = EntityState.Modified;

                    db.SaveChanges();

                    Session["chatRoomId"] = chatRoom.Id;

                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.message = "Not enough tokens";
            }
            
            return View(chatRoom);
        }


        // GET: ChatRooms/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatRoom chatRoom = db.ChatRooms.Find(id);

            if (Session["user"] != null || (int)Session["userId"] == chatRoom.CreatorID)
            {
                return RedirectToAction("Index", "Home");
            }

            if (chatRoom == null)
            {
                return HttpNotFound();
            }
            return View(chatRoom);
        }

        // POST: ChatRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ChatRoom chatRoom = db.ChatRooms.Find(id);
            db.ChatRooms.Remove(chatRoom);
            db.SaveChanges();
            return RedirectToAction("Index");
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
