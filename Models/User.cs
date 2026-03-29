using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nickname")]
        [StringLength(30)]
        public string Nickname { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; } // Хранит хэш пароля

        [Column("role")]
        [StringLength(50)]
        public string Role { get; set; } = "player";

        [Column("token")]
        [StringLength(100)]
        public string? Token { get; set; }

        [Column("email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Column("registration_date")]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [Column("email_confirmation_token")]
        [StringLength(255)]
        public string? EmailConfirmationToken { get; set; }

        [Column("email_confirmation_token_expires")]
        public DateTime? EmailConfirmationTokenExpires { get; set; }
    }
}
