using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTTN_ViettelStore.Models
{
    [Table("News")]
    public class ItemNews
    {
        [Key]
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? Hot {  get; set; }
    }
}
