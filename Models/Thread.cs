using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fora2.Models
{
    // pojedynczy watek na forum. np. w kategorii Motoryzjacja, na forum AUDI A4 mamy watek o poprawieniu osiągów
    public class Thread
    {
		public int ThreadId { get; set; }
		public int ViewsCount { get; set; }
		// ilosc 
		public int CommentsCount { get; set; }

		// krótka nazwa wątku wyswietlana na liscie watków w danym forum
		[Required]
		public string Name { get; set; }

		//  wątki przyklejone, wyświetlane zawsze na górze listy wątków. Wątki przyklejone może tworzyć tylko administrator 
		public bool Sticked { get; set; }

		// tresc danego watku, czyli np: w watku spalanie, bedzie kontekst "ej, dajcie znac ile spalaja wasze pojazdy i dlaczego tak duzo"
		[DataType(DataType.MultilineText)]
		[Required]
		public string Context { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime LastModified { get; set; }

		// przy usuwaniu wątku tak naprawde dezaktywujamy 
		public bool IsActive { get; set; }

		public virtual ApplicationUser Author { get; set; }
		public string AuthorId { get; set; }

		public int ForumId { get; set; }
		public virtual Forum Forum { get; set; }

		public virtual ICollection<ThreadMessage> ThreadMessages { get; set; }

	}
}
