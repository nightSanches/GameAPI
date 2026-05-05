using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Upgrades_cost")]
    public class UpgradesCost
    {
        public int Id { get; set; }
        [Column("Upgrade_id")]
        public int UpgradeId { get; set; }
        public int Level { get; set; }
        [Column("Price_gold")]
        public int PriceGold { get; set; }
        [Column("Price_silver")]
        public int PriceSilver { get; set; }
    }
}
