using Fora2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace Fora2.Authorization
{
	public static class IdentityManager
	{
		public static RoleManager<IdentityRole> LocalRoleManager
		{
			get
			{
				return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
			}
		}


		public static UserManager<ApplicationUser> LocalUserManager
		{
			get
			{
				return new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
			}
		}


		public static ApplicationUser GetUserByID(string userID)
		{
			ApplicationUser user = null;
			UserManager<ApplicationUser> um = LocalUserManager;

			user = um.FindById(userID);

			return user;
		}


		public static ApplicationUser GetUserByName(string email)
		{
			ApplicationUser user = null;
			UserManager<ApplicationUser> um = LocalUserManager;

			user = um.FindByEmail(email);

			return user;
		}


		public static bool RoleExists(string name)
		{
			var rm = LocalRoleManager;


			return rm.RoleExists(name);
		}
		public static dynamic GetRoles()
		{
			var rm = LocalRoleManager;

			return rm.Roles.ToList();
		}
		public static dynamic GetRoleById(string id)
		{
			var rm = LocalRoleManager;

			return rm.Roles.FirstOrDefault(r => r.Id == id);
		}


		public static bool CreateRole(string name)
		{
			var rm = LocalRoleManager;
			var idResult = rm.Create(new IdentityRole(name));

			return idResult.Succeeded;
		}


		public static bool CreateUser(ApplicationUser user, string password)
		{
			var um = LocalUserManager;
			var idResult = um.Create(user, password);

			return idResult.Succeeded;
		}


		public static bool AddUserToRole(string userId, string roleName)
		{
			var um = LocalUserManager;
			var idResult = um.AddToRole(userId, roleName);

			return idResult.Succeeded;
		}


		public static bool AddUserToRoleByUsername(string username, string roleName)
		{
			var um = LocalUserManager;

			string userID = um.FindByName(username).Id;
			var idResult = um.AddToRole(userID, roleName);

			return idResult.Succeeded;
		}


		public static void ClearUserRoles(string userId)
		{
			var um = LocalUserManager;
			var user = um.FindById(userId);
			var currentRoles = new List<IdentityUserRole>();

			currentRoles.AddRange(user.Roles);

			foreach (var role in currentRoles)
			{
				um.RemoveFromRole(userId, role.RoleId);
			}
		}

		public static bool IsUserIsAdmin(string userID)
		{
			return IsUserHaveRole(userID, "Admin");
		}
		
		public static bool IsUserHaveRole(string userID, string role)
		{
			bool haveRole;
			if (userID == null || userID == "0")
			{
				haveRole = false;
			}
			else
			{
				haveRole = LocalUserManager.IsInRole(userID, role);
			}

			return haveRole;
		}

		public static IList<string> GetUserRoles(string userID)
		{
			return LocalUserManager.GetRoles(userID);
		}
	}
}