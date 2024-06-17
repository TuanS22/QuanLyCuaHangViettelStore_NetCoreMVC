using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTTN_ViettelStore.Models
{
    [Table("Orders")]
    public class ItemOrders
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreateTime { get; set; }
        public double Price { get; set; }
        public int Status { get; set; }

    }
}
