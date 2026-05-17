namespace GameAPI.Models.Game
{
    public class GameEndResponse
    {
        public int Money { get; set; }
        public int BestScore { get; set; }
        public int Rank { get; set; }
        public int GamesPlayed { get; set; }
        public int BlocksPlaced { get; set; }
        public int PerfectBlocks { get; set; }
        public bool IsNewRecord { get; set; }
    }
}
