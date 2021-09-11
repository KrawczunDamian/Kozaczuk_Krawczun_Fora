using Fora2.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Fora2.Authorization
{
	// autoryzacja moderatora do edycji i usuwania wiaodmosci w danym forum
	public class ModeratorAuthorizeAttribute : AuthorizeAttribute // dziedziczymy po wbudowanym AuthorizeAttribute
	{
		ApplicationDbContext db;
		public ModeratorAuthorizeAttribute()
		{
			db = new ApplicationDbContext();
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext) // metoda, przy pomocy której udzielamy dostępu do akcji bądź nie
		{

			var id = (httpContext.Request.RequestContext.RouteData.Values["id"] as string)
			??
			(httpContext.Request["id"] as string);

			ThreadMessage threadMessage = db.ThreadMessages.Find(Int32.Parse(id));
			var necessaryRole = threadMessage.Thread.Forum.ModeratorRole.Name;


			if (httpContext.User.IsInRole(necessaryRole) || httpContext.User.IsInRole("Admin")) // sprawdzamy, czy aktualny user jest moderatorem danego forum lub adminem
			{
				return true; // jeśli tak - uzyskuje on dostęp do akcji
			}

			return false;
		}
	}
}