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
        [Column("Price_money")]
        public int PriceMoney { get; set; }
    }
}
