using System.Web.Mvc;
using Fora2.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Fora2.Authorization;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace Fora2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        
        public AdminController()
        {

        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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
        [HttpGet]
        public ActionResult ManageUsers()
        {
            var users = db.Users;
            return View(users);
        }
        [HttpGet]
        public ActionResult ManageRoles()
        {
            var roles = IdentityManager.GetRoles();
            return View(roles);
        }
        [HttpGet]
        public ActionResult ManageAnnouncements()
        {
            var announcements = db.Announcements;
            return View(announcements);
        }

        [HttpGet]
        public async Task<ActionResult> EditUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("NotFound");
            }
            var userRoles = await UserManager.GetRolesAsync(id);
            ViewBag.AllRoles = IdentityManager.GetRoles();
            Dictionary<string, bool> allUserRoles = new Dictionary<string, bool>();
            foreach (var item in ViewBag.AllRoles) 
            {
                allUserRoles.Add(item.Name, false);
            }
            List<string> keys = new List<string>(allUserRoles.Keys);
            foreach (var useRole in userRoles)
            {
                foreach (var key in keys)
                {
                    if (useRole == key) allUserRoles[key] = true;
                }
            }
            if(user.Rank==null)
            {
                user.Rank = new Rank();
            }
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                UserRoles = userRoles,
                AllUserRoles = allUserRoles,
                Points = user.Points,
                Rank = user.Rank.RankName
            };
            DbSet<Rank> ranks = db.Ranks;
            ViewBag.Ranks = db.Ranks;
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            ViewBag.AllRoles = IdentityManager.GetRoles();
            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return View("NotFound");
            }
            var roles = await UserManager.GetRolesAsync(user.Id);
            //First erase all the roles from the user
            var eraseRoles = await UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());
            if (!eraseRoles.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            //Then create list of users to add to user based on AllUserRoles Dictionary (dictionary is needed to make checkboxes pass true/false to role names)
            IList<string> rolesToAdd = new List<string>();
            foreach (var item in model.AllUserRoles)
            {
                if (item.Value == true) rolesToAdd.Add(item.Key);
            }
            //The list of users to add changed to array can be finally uploaded to user's Roles
            var addRolesBack = await UserManager.AddToRolesAsync(user.Id, rolesToAdd.ToArray());
            if (!addRolesBack.Succeeded)
            {
                ModelState.AddModelError("", "Can't add selected roles to user");
                return View(model);
            }
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.Points = model.Points;

            // oblicz nowa range
            int achivedRankPoints = 0;
            Rank achivedRank;
            foreach(var rank in db.Ranks)
            {
                if(rank.Requirement<= model.Points && rank.Requirement > achivedRankPoints)
                {
                    achivedRankPoints = rank.Requirement;
                    achivedRank = rank;
                    user.RankId = achivedRank.RankId;
                }
            }

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUsers");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(model);
        }
    }
}