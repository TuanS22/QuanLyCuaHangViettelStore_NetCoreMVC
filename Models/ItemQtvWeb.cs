using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTTN_ViettelStore.Models
{
	[Table("QtvWeb")]
	public class ItemQtvWeb
	{
		[Key]
		public int Id { get; set; }
		public string? Photo { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
	}
}
