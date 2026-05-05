using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Upgrades")]
    public class Upgrade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Column("Price_gold")]
        public int PriceGold { get; set; }
        [Column("Price_silver")]
        public int PriceSilver { get; set; }
    }
}
