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
    public class InChatRoomsController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: InChatRooms
        public ActionResult Index()
        {
            var inChatRooms = db.InChatRooms.Include(i => i.ChatRoom).Include(i => i.User);
            return View(inChatRooms.ToList());
        }

        // GET: InChatRooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InChatRoom inChatRoom = db.InChatRooms.Find(id);
            if (inChatRoom == null)
            {
                return HttpNotFound();
            }
            return View(inChatRoom);
        }

        // GET: InChatRooms/Create
        public ActionResult Create(Guid id)
        {
            ViewBag.ChatRoomId = new SelectList(db.ChatRooms, "Id", "Id");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username");

            InChatRoom newInChat = new InChatRoom();

            newInChat.ChatRoomId = id;
            newInChat.UserId = (int)Session["userId"];

            db.InChatRooms.Add(newInChat);
            db.SaveChanges();

            return RedirectToAction("Index", "ChatRooms");
        }

        // POST: InChatRooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ChatRoomId")] InChatRoom inChatRoom)
        {
            if (ModelState.IsValid)
            {
                db.InChatRooms.Add(inChatRoom);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ChatRoomId = new SelectList(db.ChatRooms, "Id", "Id", inChatRoom.ChatRoomId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", inChatRoom.UserId);
            return View(inChatRoom);
        }

        // GET: InChatRooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InChatRoom inChatRoom = db.InChatRooms.Find(id);
            if (inChatRoom == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChatRoomId = new SelectList(db.ChatRooms, "Id", "Id", inChatRoom.ChatRoomId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", inChatRoom.UserId);
            return View(inChatRoom);
        }

        // POST: InChatRooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ChatRoomId")] InChatRoom inChatRoom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inChatRoom).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChatRoomId = new SelectList(db.ChatRooms, "Id", "Id", inChatRoom.ChatRoomId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", inChatRoom.UserId);
            return View(inChatRoom);
        }

        // GET: InChatRooms/Delete/5
        public ActionResult Delete(int? user, Guid? room)
        {
            InChatRoom inChatRoom = db.InChatRooms.SingleOrDefault(c => ((c.UserId == user) && (c.ChatRoomId == room)));

            if(inChatRoom != null)
            {
                db.InChatRooms.Remove(inChatRoom);
            }

           
            db.SaveChanges();

            return RedirectToAction("Index", "ChatRooms");
        }

        // POST: InChatRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InChatRoom inChatRoom = db.InChatRooms.Find(id);
            db.InChatRooms.Remove(inChatRoom);
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
