using static GameAPI.Models.Authentification.FullLoginResponse;

namespace GameAPI.Models.Shop
{
    public class PurchaseResponse
    {
        public int Money { get; set; }
        public List<UserBonusDto> Bonuses { get; set; }
        public List<UserUpgradeDto> Upgrades { get; set; }
    }
}
