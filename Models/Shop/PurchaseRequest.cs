using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models.Shop
{
    public class PurchaseRequest
    {
        public string ItemType { get; set; } // "bonus", "cosmetic", "upgrade"
        public int ItemId { get; set; }
        public int Level { get; set; } // для улучшений, иначе 0
    }

}
