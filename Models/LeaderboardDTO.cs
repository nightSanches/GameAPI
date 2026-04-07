namespace GameAPI.Models
{
    public class LeaderboardEntry
    {
        public int UserId { get; set; }
        public int BestScore { get; set; }
    }

    public class LeaderboardResponse
    {
        public List<LeaderboardEntry> Top50 { get; set; }
        public int? UserBestScore { get; set; }   // null, если у пользователя нет записи в UserScores
        public int? UserPlace { get; set; }       // null, если нет записи или пользователь не авторизован
    }
}
