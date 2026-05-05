using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users_upgrades")]
    public class UserUpgrade
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        [Column("Upgrade_id")]
        public int UpgradeId { get; set; }
        public int Level { get; set; }
        public User User { get; set; }
        public Upgrade Upgrade { get; set; }
    }
}
