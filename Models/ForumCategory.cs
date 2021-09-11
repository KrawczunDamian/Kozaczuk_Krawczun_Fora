using System.Collections.Generic;

namespace Fora2.Models
{
    // kategoria wielu forów - np motoryzacja, a w niej bedzie kilka forów: motocykle, samochody osobowe, ciezarowe
    public class ForumCategory
    {
        public int ForumCategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Forum> Forum { get; set; }
    }
}
