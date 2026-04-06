using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models
{
    [Table("users_cosmetics")]
    public class UserCosmetics
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("cosmetics_id")]
        public int CosmeticsId { get; set; }
    }
}
