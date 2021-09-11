using System.ComponentModel.DataAnnotations;
namespace Fora2.Models
{
    public class ForbiddenWord
    {
        public int ForbiddenWordId { get; set; }
        [Required]
        public string Word { get; set; }

    }
}