using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Fora2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public int? RankId { get; set; }

        public virtual Rank Rank { get; set; }

        public int Points { get; set; }
        public bool IsActive { get; set; }
        // jeden użytkownik może miec wiele ról, bo np moze byc moderatorem kilku forów 
        public virtual ICollection<Role> Role { get; set; }
        
        public int MessagesPerPage { get; set; }
        public string AvatarURL { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public IDbSet<Thread> Threads { get; set; }
        public IDbSet<ThreadMessage> ThreadMessages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<ForumCategory> ForumCategory { get; set; }
        public DbSet<ForbiddenWord> ForbiddenWords { get; set; }

    }	
}