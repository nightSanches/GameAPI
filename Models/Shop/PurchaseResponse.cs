using GameAPI.Models.UserProfile;
using static GameAPI.Models.Authentification.FullLoginResponse;

namespace GameAPI.Models.Shop
{
    public class PurchaseResponse
    {
        public int Gold { get; set; }
        public int Silver { get; set; }
        public List<UserBonusDto> Bonuses { get; set; }
        public List<UserUpgradeDto> Upgrades { get; set; }
    }
}
