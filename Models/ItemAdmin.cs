using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TTTN_ViettelStore.Models
{
    [Table("Admin")]
    public class ItemAdmin
    {
        [Key]
        public int Id { get; set; }
        public string ? Photo { get; set; }
        public string ? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
