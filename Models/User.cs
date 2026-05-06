using GameAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        [Column("Registration_date")]
        public DateTime? RegistrationDate { get; set; }
        [Column("Email_confirmed")]
        public bool EmailConfirmed { get; set; }
        [Column("Email_confirmation_token")]
        public string? EmailConfirmationToken { get; set; }
        [Column("Email_confirmation_token_expires")]
        public DateTime? EmailConfirmationTokenExpires { get; set; }

        // Навигационные свойства
        public UserWallet Wallet { get; set; }
        public UserScore Score { get; set; }
        public UserGift Gift { get; set; }
        public ICollection<UserBonus> Bonuses { get; set; }
        public ICollection<UserUpgrade> Upgrades { get; set; }
    }
}
