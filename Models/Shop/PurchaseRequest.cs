using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models.Shop
{
    public class PurchaseBonusRequest
    {
        [Range(1, int.MaxValue)]
        public int BonusId { get; set; }
        // количество = 1 всегда (можно закупать по одному)
    }

    public class PurchaseCosmeticRequest
    {
        [Range(1, int.MaxValue)]
        public int CosmeticId { get; set; }
    }

    public class PurchaseUpgradeRequest
    {
        [Range(1, int.MaxValue)]
        public int UpgradeId { get; set; }
    }

}
