using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Fora2.Authorization;
using Fora2.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Fora2.Controllers
{
    public class ThreadsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(int? id, string searchWord)
        {
            ViewBag.UserIsAdmin = IdentityManager.IsUserIsAdmin(User.Identity.GetUserId());

            if (id == null)
            {
                var threads = db.Threads.Include(t => t.Author).Include(t => t.Forum);

                return View(threads.ToList());
            }
            else if (searchWord == null || searchWord=="")
            {
                var model = from t in db.Threads
                            where t.Forum.ForumId == id
                            select t;

                ViewBag.CurrentForumId = id;
                ViewBag.CurrentForum = db.Forums.FirstOrDefault(f => f.ForumId == (int)id);

                return View(model.ToList());
            }
            else // gdy chce dodatkowo wyswietlic wyszukiwane słowo
            {
                var model = from t in db.Threads
                            where t.Forum.ForumId == id
                            select t;

                ViewBag.CurrentForumId = id;
                ViewBag.CurrentForum = db.Forums.FirstOrDefault(f => f.ForumId == (int)id);

                var splitedSearch = searchWord.Split(' ');
                
                // napis sklada sie z 3 czlonow w tym 2 czlon to or lub and 
                if(splitedSearch.Length == 3 && (searchWord.ToLower().Contains("or") || searchWord.ToLower().Contains("and") || searchWord.ToLower().Contains("not")))
                {
                    string firstWord = splitedSearch[0].ToLower();
                    string secondWord = splitedSearch[2].ToLower();
                    switch (splitedSearch[1].ToLower())
                    {
                        case "and":
                            {

                                ViewBag.SearchedMessages = db.ThreadMessages.Where(m => m.Thread.ForumId == id && 
                                                                        ( m.Context.ToLower().Contains(firstWord) 
                                                                        &&
                                                                        m.Context.ToLower().Contains(secondWord)) ).ToList<ThreadMessage>();
                                break;
                            }
                        case "or":
                            {
                                ViewBag.SearchedMessages = db.ThreadMessages.Where(m => m.Thread.ForumId == id &&
                                                                        (m.Context.ToLower().Contains(firstWord)
                                                                        ||
                                                                        m.Context.ToLower().Contains(secondWord))).ToList<ThreadMessage>();

                                break;
                            }
                        case "not":
                            {
                                ViewBag.SearchedMessages = db.ThreadMessages.Where(m => m.Thread.ForumId == id &&
                                                                        (m.Context.ToLower().Contains(firstWord)
                                                                        &&
                                                                        !m.Context.ToLower().Contains(secondWord))).ToList<ThreadMessage>();

                                break;
                            }
                    }   
                    
                    ViewBag.SearchedWord = searchWord;
                }
                
                else
                {
                    ViewBag.SearchedMessages = db.ThreadMessages.Where(m => m.Thread.ForumId == id && m.Context.ToLower().Contains(searchWord.ToLower())).ToList<ThreadMessage>();    
                }

                ViewBag.SearchedWord = searchWord;

                return View(model.ToList());
            }

        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(int? id, string searchWord )
        {
            if(searchWord!="")
            return Index(id, searchWord);
            else
                return Index(id, null);
        }


        // GET: Threads/Details/5
        public ActionResult Details(int? id, int? page)
        {
            ViewBag.ForbiddenWords = TempData["usedForbiddenWords"];

            if (TempData["wasForbiddenWord"] != null)
                ViewBag.IsContainsForbiddenWords = TempData["wasForbiddenWord"];
            else
                ViewBag.IsContainsForbiddenWords = false;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thread thread = db.Threads.Find(id);
            var userID =  User.Identity.GetUserId();
            
            //jezeli uzytkownik jest anonimowy i forum nie pozwala na komentarze anoniowych
            if (!thread.Forum.IsAnonymousCanComment && userID == null)
            {
                ViewBag.AnonymousCanComment = false;
            }
            else
            {
                ViewBag.AnonymousCanComment = true;
            }
            

            bool userIsModerator = IdentityManager.IsUserHaveRole(userID, thread.Forum.ModeratorRole.Name);
            bool userIsAdmin = IdentityManager.IsUserIsAdmin(userID);
            //jezeli wchodzi moderator to pokaz mu opcje edycji i usuwania wiadomosci watkow - admin tez jest moderatorem   
            // a gdy admin to dodatkowo moze edytowac sam watek
            if (userIsAdmin)
            {
                ViewBag.UserIsAdmin = true;
                ViewBag.UserIsModerator = true;
            }
            else if (userIsModerator)
            {
                ViewBag.UserIsAdmin = false;
                ViewBag.UserIsModerator = true;
            }
            else
            {
                ViewBag.UserIsAdmin = false;
                ViewBag.UserIsModerator = false;
            }
            
            thread.ViewsCount++;
            db.SaveChanges();
            ViewBag.Thread = thread;
            ViewBag.Messages = thread.ThreadMessages;
            if (thread == null)
            {
                return HttpNotFound();
            }
            var user = IdentityManager.GetUserByID(userID);
            if(user!=null) return View(thread.ThreadMessages.ToList().ToPagedList(page ?? 1,user.MessagesPerPage));
            return View(thread.ThreadMessages.ToList().ToPagedList(page ?? 1, 5));
        }

        // stick or unstick
        [Authorize(Roles = "Admin")]
        public ActionResult Stick(int? id)
        {
            // przypnij (zmodyfikuj watek) i wroc do strony index
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thread thread = db.Threads.Find(id);
            if (thread == null)
            {
                return HttpNotFound();
            }

             var currentState = thread.Sticked;
             thread.Sticked = !currentState;
             db.Entry(thread).State = EntityState.Modified;
             db.SaveChanges();
            
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AnonymousAuthorize]
        public ActionResult Details(int? id, string newMessageContext="")
        {
            // sprawdz zakazane słowa
            bool messageContainsForbiddenWord = false;
            List<string> forbiddenWordsInMessage = new List<string>();
            foreach(var forbiddenWord in db.ForbiddenWords)
            {
                if(newMessageContext.ToLower().Contains(forbiddenWord.Word.ToLower()))
                {
                    forbiddenWordsInMessage.Add(forbiddenWord.Word);
                    messageContainsForbiddenWord = true;
                }
            }


            if (newMessageContext!= null && newMessageContext!="" && !messageContainsForbiddenWord)
            {

                var newThreadMessage = new ThreadMessage();

                // mozna dodac opcje, ze gdy jest niezalogowany to wstawia jakiegos użytkownika "anonimowego" z bazy
                string userID = User.Identity.GetUserId();
                if (userID == null)
                {
                    // przypisuje anonimowego uzytkownika
                    string annomousId = IdentityManager.GetUserByName("anonymous@anonymous.pl").Id;
                    newThreadMessage.AuthorId = annomousId;
                    userID = annomousId;
                }
                else
                {
                    newThreadMessage.AuthorId = userID;
                }

                newThreadMessage.Context = newMessageContext;
                newThreadMessage.CreateDate = DateTime.Now;
                newThreadMessage.LastModified = DateTime.Now;
                newThreadMessage.ThreadId = (int)id;
                Thread thread = db.Threads.Find(id);
                thread.CommentsCount++;
                thread.LastModified = DateTime.Now;
                db.ThreadMessages.Add(newThreadMessage);
                db.SaveChanges();

                // dodaj i przelicz punkty
                var user = db.Users.Find(userID);
                user.Points += 10;

                // przelicza mozliwie nową range
                int achivedRankPoints = 0;
                Rank achivedRank;
                foreach (var rank in db.Ranks)
                {
                    if (rank.Requirement <= user.Points && rank.Requirement > achivedRankPoints)
                    {
                        achivedRankPoints = rank.Requirement;
                        achivedRank = rank;
                        user.RankId = achivedRank.RankId;
                    }
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            TempData["wasForbiddenWord"] = messageContainsForbiddenWord;
            TempData["usedForbiddenWords"] = forbiddenWordsInMessage;

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: Threads/Create
        [Authorize]
        public ActionResult Create(int? id)
        {
            if (id != null)
            {
               // gdy true to chowamy czesc elementów takich jak forum id 
                ViewBag.SelectedForum = true;

                var newThread = new Thread();

                var userID = User.Identity.GetUserId();
                

                newThread.ForumId = db.Forums.SingleOrDefault(x => x.ForumId == id).ForumId;
                newThread.Forum = db.Forums.SingleOrDefault(x => x.ForumId == id);
                return View(newThread);
            }
            ViewBag.SelectedForum = false;

            return RedirectToAction("Index", "Home");
        }

        // POST: Threads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ThreadId,ViewsCount,CommentsCount,Name,Sticked,Context,CreateDate,LastModified,IsActive,AuthorId,ForumId")] Thread thread)
        {
            if (ModelState.IsValid)
            {
                var userID = User.Identity.GetUserId();
                if(userID==null)
                {
                    thread.AuthorId = IdentityManager.GetUserByName("anonymous@anonymous.pl").Id;
                    //przypisz uzytkownika anonimowego
                }
                else
                {
                    thread.AuthorId = userID;
                }

                thread.ViewsCount = 0;
                thread.CreateDate = DateTime.Now;
                thread.LastModified = DateTime.Now;
                thread.IsActive = true;


                db.Threads.Add(thread);
                db.SaveChanges();
                return RedirectToAction("Index", "Threads", new { id = thread.ForumId });
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email", thread.AuthorId);
            ViewBag.ForumId = new SelectList(db.Forums, "ForumId", "Name", thread.ForumId);
            return View(thread);
        }

        // GET: Threads/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thread thread = db.Threads.Find(id);
            if (thread == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email", thread.AuthorId);
            ViewBag.ForumId = new SelectList(db.Forums, "ForumId", "Name", thread.ForumId);
            return View(thread);
        }

        // POST: Threads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ThreadId,ViewsCount,CommentsCount,Name,Sticked,Context,CreateDate,LastModified,IsActive,AuthorId,ForumId")] Thread thread)
        {
            if (ModelState.IsValid)
            {
                thread.LastModified = DateTime.Now;
                db.Entry(thread).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details","Threads", new { id = thread.ThreadId });
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "Email", thread.AuthorId);
            ViewBag.ForumId = new SelectList(db.Forums, "ForumId", "Name", thread.ForumId);
            return View(thread);
        }

        // GET: Threads/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thread thread = db.Threads.Find(id);
            if (thread == null)
            {
                return HttpNotFound();
            }
            return View(thread);
        }

        // POST: Threads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Thread thread = db.Threads.Find(id);
            db.Threads.Remove(thread);
            thread.CommentsCount--;
            db.SaveChanges();
            return RedirectToAction("Index","Threads",new { id = thread.ForumId});
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
