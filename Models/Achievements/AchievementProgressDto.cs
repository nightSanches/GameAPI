namespace GameAPI.Models.Achievements
{
    /// <summary>
    /// DTO для передачи прогресса конкретного достижения
    /// </summary>
    public class AchievementProgressDto
    {
        /// <summary>
        /// ID достижения
        /// </summary>
        public int AchievementId { get; set; }

        /// <summary>
        /// Текущий прогресс игрока по этому достижению
        /// </summary>
        public int CurrentProgress { get; set; }
    }
}
