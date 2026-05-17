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
        public int MaxFloor { get; set; }
        public int PerfectStreak { get; set; }
    }
}
