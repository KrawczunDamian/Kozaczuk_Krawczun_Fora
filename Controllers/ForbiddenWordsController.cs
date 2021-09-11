using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Fora2.Models;

namespace Fora2.Controllers
{
    public class ForbiddenWordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ForbiddenWords
        public ActionResult Index()
        {
            return View(db.ForbiddenWords.ToList());
        }

        // GET: ForbiddenWords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForbiddenWord forbiddenWord = db.ForbiddenWords.Find(id);
            if (forbiddenWord == null)
            {
                return HttpNotFound();
            }
            return View(forbiddenWord);
        }

        // GET: ForbiddenWords/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ForbiddenWords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ForbiddenWordId,Word")] ForbiddenWord forbiddenWord)
        {
            if (ModelState.IsValid)
            {
                db.ForbiddenWords.Add(forbiddenWord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(forbiddenWord);
        }

        // GET: ForbiddenWords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForbiddenWord forbiddenWord = db.ForbiddenWords.Find(id);
            if (forbiddenWord == null)
            {
                return HttpNotFound();
            }
            return View(forbiddenWord);
        }

        // POST: ForbiddenWords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ForbiddenWordId,Word")] ForbiddenWord forbiddenWord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(forbiddenWord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(forbiddenWord);
        }

        // GET: ForbiddenWords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ForbiddenWord forbiddenWord = db.ForbiddenWords.Find(id);
            if (forbiddenWord == null)
            {
                return HttpNotFound();
            }
            return View(forbiddenWord);
        }

        // POST: ForbiddenWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ForbiddenWord forbiddenWord = db.ForbiddenWords.Find(id);
            db.ForbiddenWords.Remove(forbiddenWord);
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
