using GameAPI.Models.UserProfile;

namespace GameAPI.Models.Shop
{
    public class PurchaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public WalletInfo NewWallet { get; set; }
        public int? NewQuantity { get; set; }     // для бонусов
        public int? NewLevel { get; set; }        // для улучшений
        public int? PurchasedCosmeticId { get; set; } // для косметики
    }
}
