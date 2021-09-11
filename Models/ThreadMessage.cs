using System;

namespace Fora2.Models
{
	/// <summary>
	/// Jest to pojedynczy rekord komentarza w danym wątku - pewnego radzaju odpowiedz na Thread
	/// </summary>
	public class ThreadMessage
	{
		public ThreadMessage() { }

		public int ThreadMessageId { get; set; }

		// tresc wiadomosci/komentarza do danego watku
		public string Context { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime LastModified { get; set; }

		public virtual ApplicationUser Author { get; set; }
		public string AuthorId { get; set; }

		public int ThreadId { get; set; }
		public virtual Thread Thread { get; set; }
	}
}
