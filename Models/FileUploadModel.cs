using System.ComponentModel.DataAnnotations;

namespace Fora2.Models
{
    public class FileUploadModel
	{
		[DataType(DataType.Upload)]
		[Display(Name ="Upload File")]
		[Required(ErrorMessage ="Choose a file to upload.")]
		public string file { get; set; }
	}
}
