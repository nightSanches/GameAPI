using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Users_wallet")]
    public class UserWallet
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public User User { get; set; }
    }
}
