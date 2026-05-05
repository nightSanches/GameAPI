using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Cosmetics")]
    public class Cosmetic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Column("Price_silver")]
        public int PriceSilver { get; set; }
    }
}
