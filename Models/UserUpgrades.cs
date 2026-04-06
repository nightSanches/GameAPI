using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models
{
    [Table("users_upgrades")]
    public class UserUpgrades
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("gold")]
        public int Gold { get; set; }

        [Required]
        [Column("silver")]
        public int Silver { get; set; }

        [Required]
        [Column("up_crane")]
        public int UpCrane { get; set; }

        [Required]
        [Column("up_base")]
        public int UpBase { get; set; }

        [Required]
        [Column("up_extra_gold")]
        public int UpExtraGold { get; set; }

        [Required]
        [Column("up_extra_silver")]
        public int UpExtraSilve { get; set; }

        [Required]
        [Column("up_extra_mul")]
        public int UpExtraMul { get; set; }
    }
}
