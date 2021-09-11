using System.Linq;
using System.Web.Mvc;
using Fora2.Models;

namespace Fora2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            AnnouncementsPlusCategoriesModel mymodel = new AnnouncementsPlusCategoriesModel();
            mymodel.Announcements = db.Announcements.ToList();
            mymodel.Categories = db.ForumCategory.ToList();
            ViewBag.RegisteredUsers = db.Users.Count();
            return View(mymodel);
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