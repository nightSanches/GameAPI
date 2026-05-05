using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Bonuses")]
    public class Bonus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Column("Price_gold")]
        public int PriceGold { get; set; }
    }
}
