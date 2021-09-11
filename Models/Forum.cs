using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Fora2.Models
{
    public class Forum
    {
        public int ForumId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Thread> Threads { get; set; }
        public int ForumCategoryId { get; set; }
        // dane forum na swoj typ moderatora - przy towrzeniu jako admin nowego forum zaznaczamy jaki typ użytkowników będzie miało opcje moderatorskie
        public string ModeratorRoleId { get; set; } 
        public virtual IdentityRole ModeratorRole { get; set; }
        public virtual ForumCategory ForumCategory { get; set; }

        public bool IsAnonymousCanComment { get; set; }
    }
}
