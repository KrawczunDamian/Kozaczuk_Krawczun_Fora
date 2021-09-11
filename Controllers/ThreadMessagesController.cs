using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Fora2.Authorization;
using Fora2.Models;

namespace Fora2.Controllers
{
    public class ThreadMessagesController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: ThreadMessages
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var threadMessages = db.ThreadMessages.Include(t => t.Author).Include(t => t.Thread);
            return View(threadMessages.ToList());
        }

        // GET: ThreadMessages/Details/5
        [ModeratorAuthorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThreadMessage threadMessage = db.ThreadMessages.Find(id);
            if (threadMessage == null)
            {
                return HttpNotFound();
            }
            return View(threadMessage);
        }

        // GET: ThreadMessages/Create
        // tworzyc moze czasmi annomous z zaleznosci od ustawienia forum
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email");
            ViewBag.ThreadId = new SelectList(db.Threads, "ThreadId", "Name");
            return View();
        }

        // POST: ThreadMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ThreadMessageId,Context,CreateDate,LastModified,AuthorId,ThreadId")] ThreadMessage threadMessage)
        {
            if (ModelState.IsValid)
            {
                db.ThreadMessages.Add(threadMessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email", threadMessage.AuthorId);
            ViewBag.ThreadId = new SelectList(db.Threads, "ThreadId", "Name", threadMessage.ThreadId);
            return View(threadMessage);
        }

        // GET: ThreadMessages/Edit/5
        // admin i moderator
        [ModeratorAuthorize]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThreadMessage threadMessage = db.ThreadMessages.Find(id);
            threadMessage.LastModified = DateTime.Now;
            if (threadMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email", threadMessage.AuthorId);
            ViewBag.ThreadId = new SelectList(db.Threads, "ThreadId", "Name", threadMessage.ThreadId);
            return View(threadMessage);
        }

        // POST: ThreadMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModeratorAuthorize]
        public ActionResult Edit([Bind(Include = "ThreadMessageId,Context,CreateDate,LastModified,AuthorId,ThreadId")] ThreadMessage threadMessage)
        {
            if (ModelState.IsValid)
            {
                threadMessage.LastModified = DateTime.Now;
                db.Entry(threadMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Threads", new { id = threadMessage.ThreadId});
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email", threadMessage.AuthorId);
            ViewBag.ThreadId = new SelectList(db.Threads, "ThreadId", "Name", threadMessage.ThreadId);
            return View(threadMessage);
        }

        // GET: ThreadMessages/Delete/5
        // admin i moderator
        [ModeratorAuthorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThreadMessage threadMessage = db.ThreadMessages.Find(id);
            if (threadMessage == null)
            {
                return HttpNotFound();
            }
            return View(threadMessage);
        }

        // POST: ThreadMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ModeratorAuthorize]
        public ActionResult DeleteConfirmed(int id)
        {
            ThreadMessage threadMessage = db.ThreadMessages.Find(id);
            db.ThreadMessages.Remove(threadMessage);
            db.Threads.Find(threadMessage.ThreadId).CommentsCount--;
            db.SaveChanges();
            
            return RedirectToAction("Details","Threads",new { id = threadMessage.ThreadId });
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
