using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTTN_ViettelStore.Models
{
    [Table("Slides")]
    public class ItemSlides
    {
        [Key]
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Info { get; set; }
        public string? Link { get; set; }
    }
}
