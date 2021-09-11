using System;

namespace Fora2.Models
{
    /// <summary>
    /// Ogłoszenia - ogłoszenia administracyjne wyświetlane na stronie głównej forum – 2 pkt.
    /// </summary>
    public class Announcement
    {
		public int Id { get; set; }

		public string Content { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime ExpirationDate { get; set; }
	}
}
