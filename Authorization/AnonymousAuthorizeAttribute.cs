using Fora2.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Fora2.Authorization
{
	// autoryzacja moderatora do edycji i usuwania wiaodmosci w danym forum
	public class AnonymousAuthorizeAttribute : AuthorizeAttribute // dziedziczymy po wbudowanym AuthorizeAttribute
	{
		ApplicationDbContext db;
		public AnonymousAuthorizeAttribute()
		{
			db = new ApplicationDbContext();
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext) // metoda, przy pomocy której udzielamy dostępu do akcji bądź nie
		{
			if (httpContext.User.Identity.IsAuthenticated) // jezeli uzytkownik jest zalogowany to zawsze zezwalaj
			{
				return true; 
			}
			else
			{
				// wez id watku
				var id = (httpContext.Request.RequestContext.RouteData.Values["id"] as string)
				??
				(httpContext.Request["id"] as string);

				//znajdz watek
				Thread thread = db.Threads.Find(Int32.Parse(id));

				//sprawdz czy forum tego watku zezwala na anonimowe wiadomosci
				return thread.Forum.IsAnonymousCanComment;
			}
		}
	}
}