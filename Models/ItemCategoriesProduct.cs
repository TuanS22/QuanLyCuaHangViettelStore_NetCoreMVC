using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTTN_ViettelStore.Models
{
    [Table("CategoriesProducts")]
    public class ItemCategoriesProduct
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }

    }
}
