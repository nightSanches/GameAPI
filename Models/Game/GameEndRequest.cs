namespace GameAPI.Models.Game
{
    public class GameEndRequest
    {
        public int Score { get; set; }
        public int GoldEarned { get; set; }
        public int BlocksPlaced { get; set; }
        public int PerfectBlocks { get; set; }
    }
}
