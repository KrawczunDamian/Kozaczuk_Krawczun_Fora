using System.Collections.Generic;

namespace Fora2.Models
{
    public class AnnouncementsPlusCategoriesModel
	{
		public IEnumerable<Announcement> Announcements { get; set; }
		public IEnumerable<ForumCategory> Categories { get; set; }
	}
}
