using System.Collections.Generic;

namespace Fora2.Models
{
    // klasa reprezentujaca role/ uprawnienia - np. admin, moderator, user
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        // liste użytkowników z daną rolą
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
