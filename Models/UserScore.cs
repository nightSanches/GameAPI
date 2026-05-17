using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users_score")]
    public class UserScore
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        [Column("District_Id")]
        public int DistrictId { get; set; }
        [Column("Best_score")]
        public int BestScore { get; set; }
        public User User { get; set; }
        public District District { get; set; }
    }
}
