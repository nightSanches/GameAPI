using System.Runtime;

namespace GameAPI.Models.UserProfile
{
    // Ответ с полным состоянием пользователя (синхронизация после логина или важных действий)
    public class UserProfileResponse
    {
        public UserInfo User { get; set; }
        public WalletInfo Wallet { get; set; }
        public ScoreInfo Score { get; set; }
        public GiftInfo Gift { get; set; }
        public List<BonusInfo> Bonuses { get; set; }
        public List<UpgradeInfo> Upgrades { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    public class WalletInfo
    {
        public int Gold { get; set; }
    }

    public class ScoreInfo
    {
        public int BestScore { get; set; }
    }

    public class GiftInfo
    {
        public DateTime? LastBonusDt { get; set; }
        public int SecondsUntilNextGift { get; set; } // рассчитано сервером
    }

    public class BonusInfo
    {
        public int BonusId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpgradeInfo
    {
        public int UpgradeId { get; set; }
        public int Level { get; set; }
    }
}
