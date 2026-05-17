using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users_wallet")]
    public class UserWallet
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        public int Money { get; set; }
        public int Reputation { get; set; }
        
        // Навигационные свойства
        public User User { get; set; }
    }
}
