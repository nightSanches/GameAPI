using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models
{
    [Table("users_gifts")]
    public class UserGifts
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("last_bonus_dt")]
        public DateTime? LastBonusDT { get; set; }
    }
}
