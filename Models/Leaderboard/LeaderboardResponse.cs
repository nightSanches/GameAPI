namespace GameAPI.Models.Leaderboard
{
    public class LeaderboardResponse
    {
        public List<LeaderboardEntry> TopPlayers { get; set; }
        public LeaderboardEntry CurrentPlayerEntry { get; set; } // null если игрок не авторизован
    }

    public class LeaderboardEntry
    {
        public int Position { get; set; }      // место с учётом разделения одинаковых очков
        public string Nickname { get; set; }
        public int BestScore { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
