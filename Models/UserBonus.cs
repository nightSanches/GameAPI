using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("User_bonuses")]
    public class UserBonus
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        [Column("Bonus_id")]
        public int BonusId { get; set; }
        public int Quantity { get; set; }
        public User User { get; set; }
        public Bonus Bonus { get; set; }
    }
}
