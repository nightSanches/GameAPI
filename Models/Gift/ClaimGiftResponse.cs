using GameAPI.Models.UserProfile;

namespace GameAPI.Models.Gift
{
    public class ClaimGiftResponse
    {
        public int GoldReceived { get; set; }
        public int SilverReceived { get; set; }
        public WalletInfo NewWallet { get; set; }
        public DateTime NextGiftAvailableUtc { get; set; }
        public int SecondsUntilNextGift { get; set; }
    }
}
