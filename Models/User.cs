using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fora2.Models
{
    public class User
    {
		public int UserId { get; set; }
		public string Username { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public int RankId { get; set; }
		public virtual Rank Rank { get; set; }

		//awatary w profilu użytkownika. Ograniczenie na rozdzielczość i wielkość (w kB) wgrywanych obrazków
		public byte[] Avatar { get; set; }
		public bool IsActive { get; set; }
		// jeden użytkownik może miec wiele ról, bo np moze byc moderatorem kilku forów 
		public virtual ICollection<Role> Role { get; set; }


	}
}