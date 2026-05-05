using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users_cosmetics")]
    public class UserCosmetic
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        [Column("Cosmetic_id")]
        public int CosmeticId { get; set; }
        public User User { get; set; }
        public Cosmetic Cosmetic { get; set; }
    }
}
