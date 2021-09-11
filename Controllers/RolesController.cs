using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fora2.Models;
using Fora2.Authorization;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace Fora2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleStore<IdentityRole> roleStore;
        private RoleManager<IdentityRole> roleManager;
        private ApplicationUserManager _userManager;

        public RolesController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            this.roleStore = new RoleStore<IdentityRole>(db);
            this.roleManager = new RoleManager<IdentityRole>(roleStore);
        }
        public RolesController()
        {
            this.roleStore = new RoleStore<IdentityRole>(db);
            this.roleManager = new RoleManager<IdentityRole>(roleStore);
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] IdentityRole role )
        {
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageRoles", "Admin");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(roleManager.FindById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] IdentityRole role)
        {
            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageRoles", "Admin");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(role);
        }
        // GET: Roles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = roleManager.FindById(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var role = roleManager.FindById(id);
            var newRole = IdentityManager.GetRoleById("1");
            foreach (var item in db.Forums)
            {
                if(item.ModeratorRoleId == id)
                {
                    var forum = db.Forums.Find(item.ForumId);
                    forum.ModeratorRole = newRole;
                    forum.ModeratorRoleId = newRole.Id;
                    db.Entry(forum).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            foreach (var item in db.Users)
            {
                UserManager.RemoveFromRole(item.Id, role.Name);
            }
            var result = roleManager.Delete(role);
            return RedirectToAction("ManageRoles", "Admin");
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
