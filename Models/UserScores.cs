using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models
{
    [Table("users_score")]
    public class UserScores
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("best_score")]
        public int BestScore { get; set; }
    }
}
