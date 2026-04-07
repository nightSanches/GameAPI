namespace GameAPI.Models
{
    public class BonusClaimResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime? LastBonusDT { get; set; }  // актуальная дата последнего бонуса
    }
}
