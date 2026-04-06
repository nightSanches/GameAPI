using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("users_bonuses")]
    public class UserBonuses
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("bonus_stabilizer")]
        public int BonusStabilizer { get; set; }

        [Required]
        [Column("bonus_alignment")]
        public int BonusAlignment { get; set; }

        [Required]
        [Column("bonus_insurance")]
        public int BonusInsurance { get; set; }
    }
}
