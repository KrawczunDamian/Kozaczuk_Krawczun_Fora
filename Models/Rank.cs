namespace Fora2.Models
{
	public class Rank
	{
		public int RankId { get; set; }
		// wymagana ilosc punktow do otrzymania tej rangi 
		public int Requirement { get; set; }
		public string RankName { get; set; }
	}
}
