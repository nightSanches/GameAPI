using static GameAPI.Models.Authentification.FullLoginResponse;

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
        public List<UserBonusDto> Bonuses { get; set; }
    }

    /// <summary>
    /// DTO для бонуса пользователя. Используется в GameEndResponse.
    /// </summary>
    public class UserBonusDto
    {
        public int BonusId { get; set; }
        public int Quantity { get; set; }
    }
}
