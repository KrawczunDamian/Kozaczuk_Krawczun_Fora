using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Fora2.Authorization;
using Fora2.Models;
using Microsoft.AspNet.Identity;

namespace Fora2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ForaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [AllowAnonymous]
        public ActionResult Index(int? id)
        {
            ViewBag.UserIsAdmin = IdentityManager.IsUserIsAdmin(User.Identity.GetUserId());

            var x = id;
            if (id == null)
            {
                var forums = db.Forums.Include(f => f.ForumCategory);
                ViewBag.IsExcatForumCategorySelected = false;
                return View(forums.ToList());
            }
            else
            {
                var model = from f in db.Forums
                            where f.ForumCategory.ForumCategoryId == id
                            select f;

                ViewBag.SelectedForumId = (int)id;
                ViewBag.IsExcatForumCategorySelected = true;
                ViewBag.SelectedForumCategoryName = db.ForumCategory.FirstOrDefault(f => f.ForumCategoryId == (int)id).Name;
               

                return View(model.ToList());
            }
        }

        // GET: Fora/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = db.Forums.Find(id);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forum);
        }
        // GET: Fora/Create
        public ActionResult Create(int? id)
        {
            //var identityManager = new IdentityManager();
            ViewBag.ModeratorRoleId = new SelectList(IdentityManager.GetRoles(), "Id", "Name");
            

            //jezeli w URL nie ma ID to na stronie ma byc ddl z wyborem kategorii fora
            if (id == null)
            {
                ViewBag.PreselectedCategory = false;
                ViewBag.ForumCategoryId = new SelectList(db.ForumCategory, "ForumCategoryId", "Name");
                return View();
            }
            // w przeciwnym wypadku zmien tytul i preselect kategorie
            else
            {
                ViewBag.PreselectedCategory = true;
                var newForum = new Forum();
                newForum.ForumCategory = db.ForumCategory.FirstOrDefault(c => c.ForumCategoryId == (int)id);
                newForum.ForumCategoryId = (int)id;

                ViewBag.ForumCategoryId = new SelectList(db.ForumCategory, "ForumCategoryId", "Name");
                return View(newForum);
            }
            
        }

        // POST: Fora/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ForumId,Name,ForumCategoryId,ModeratorRoleId")] Forum forum)
        {
            if (ModelState.IsValid)
            {
                db.Forums.Add(forum);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = forum.ForumCategoryId});
            }

            ViewBag.ForumCategoryId = new SelectList(db.ForumCategory, "ForumCategoryId", "Name", forum.ForumCategoryId);
            return View(forum);
        }

        // GET: Fora/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = db.Forums.Find(id);
            if (forum == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModeratorRoleId = new SelectList(IdentityManager.GetRoles(), "Id", "Name");
            ViewBag.ForumCategoryId = new SelectList(db.ForumCategory, "ForumCategoryId", "Name", forum.ForumCategoryId);
            return View(forum);
        }

        // POST: Fora/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ForumId,Name,ForumCategoryId,ModeratorRoleId, IsAnonymousCanComment")] Forum forum)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forum).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ForumCategoryId = new SelectList(db.ForumCategory, "ForumCategoryId", "Name", forum.ForumCategoryId);
            return View(forum);
        }

        // GET: Fora/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Forum forum = db.Forums.Find(id);
            if (forum == null)
            {
                return HttpNotFound();
            }
            return View(forum);
        }

        // POST: Fora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Forum forum = db.Forums.Find(id);
            db.Forums.Remove(forum);
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
