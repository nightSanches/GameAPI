namespace GameAPI.Models
{
    public class UserDataSaveRequest
    {
        public UserBonusesDto? Bonuses { get; set; }
        public List<int>? CosmeticsIds { get; set; }
        public UserGiftsDto? Gifts { get; set; }
        public UserScoresDto? Scores { get; set; }
        public UserUpgradesDto? Upgrades { get; set; }
    }

    public class UserBonusesDto
    {
        public int BonusStabilizer { get; set; }
        public int BonusAlignment { get; set; }
        public int BonusInsurance { get; set; }
    }

    public class UserGiftsDto
    {
        public DateTime? LastBonusDT { get; set; }
    }

    public class UserScoresDto
    {
        public int BestScore { get; set; }
    }

    public class UserUpgradesDto
    {
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int UpCrane { get; set; }
        public int UpBase { get; set; }
        public int UpExtraGold { get; set; }
        public int UpExtraSilver { get; set; }
        public int UpExtraMul { get; set; }
    }

    public class UserDataResponse
    {
        public UserBonusesDto Bonuses { get; set; }
        public List<int> CosmeticsIds { get; set; }
        public UserGiftsDto Gifts { get; set; }
        public UserScoresDto Scores { get; set; }
        public UserUpgradesDto Upgrades { get; set; }
    }
}
