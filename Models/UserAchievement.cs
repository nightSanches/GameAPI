using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users_achievements")]
    public class UserAchievement
    {
        public int Id { get; set; }
        [Column("User_Id")]
        public int UserId { get; set; }
        [Column("Achievement_Id")]
        public int AchievementId { get; set; }
        [Column("Current_Progress")]
        public int CurrentProgress { get; set; }
        [Column("Is_Unlocked")]
        public bool IsUnlocked { get; set; }
        
        // Навигационные свойства
        public User User { get; set; }
        public Achievement Achievement { get; set; }
    }
}
