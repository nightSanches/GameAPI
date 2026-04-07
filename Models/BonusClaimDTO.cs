namespace GameAPI.Models
{
    public class BonusClaimResponse
    {
        public bool Success { get; set; }
        public int? GoldEarned { get; set; }
        public int? SilverEarned { get; set; }
        public UserDataResponse? UserData { get; set; }
        public string? Message { get; set; }
    }
}
