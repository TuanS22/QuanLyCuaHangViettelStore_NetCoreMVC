using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTTN_ViettelStore.Models
{
    [Table("Tags")]
    public class ItemTags
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
