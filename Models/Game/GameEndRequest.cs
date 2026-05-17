namespace GameAPI.Models.Game
{
    public class GameEndRequest
    {
        public int Score { get; set; }
        public int MoneyEarned { get; set; }
        public int ReputationEarned { get; set; }
        public int DistrictId { get; set; }
        public int BlocksPlaced { get; set; }
        public int PerfectBlocks { get; set; }

        /// <summary>
        /// Прогресс достижений для различных типов условий.
        /// Ключ - тип условия (например, "max_floor", "perfect_streak", "games_played"),
        /// Значение - текущее значение прогресса.
        /// </summary>
        public Dictionary<string, int> AchievementProgresses { get; set; } = new();
    }
}
