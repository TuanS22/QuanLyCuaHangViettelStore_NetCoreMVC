using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TTTN_ViettelStore.Models
{
    [Table("Adv")]
    public class ItemAdv
    {
        [Key]
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string? Name { get; set; }
        public int Position { get; set; }

    }
}
