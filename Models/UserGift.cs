using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users_gifts")]
    public class UserGift
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        [Column("Last_bonus_dt")]
        public DateTime? LastBonusDt { get; set; }
        public User User { get; set; }
    }
}
