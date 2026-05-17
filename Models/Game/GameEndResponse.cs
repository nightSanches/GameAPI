namespace GameAPI.Models.Game
{
    public class GameEndResponse
    {
        public int Money { get; set; }
        public int Reputation { get; set; }
        public int GamesPlayed { get; set; }
        public int BlocksPlaced { get; set; }
        public int PerfectBlocks { get; set; }
        public bool IsNewRecord { get; set; }
        public List<ScoreByDistrictDto> ScoresByDistrict { get; set; }
        public List<DistrictRankDto> DistrictRanks { get; set; }
        public List<UserAchievementDto> Achievements { get; set; }
    }

    public class ScoreByDistrictDto
    {
        public int DistrictId { get; set; }
        public int BestScore { get; set; }
    }

    public class DistrictRankDto
    {
        public int DistrictId { get; set; }
        public int BestScore { get; set; }
        public int Rank { get; set; }
    }

    public class UserAchievementDto
    {
        public int AchievementId { get; set; }
        public int CurrentProgress { get; set; }
        public bool IsUnlocked { get; set; }
    }
}
