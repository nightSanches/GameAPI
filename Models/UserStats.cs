using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("User_stats")]
    public class UserStats
    {
        public int Id { get; set; }
        [Column("User_id")]
        public int UserId { get; set; }
        [Column("Games_played_count")]
        public int GamesPlayedCount { get; set; }
        [Column("Blocks_placed_count")]
        public int BlocksPlacedCount { get; set; }
        [Column("IBlocks_placed_count")]
        public int IBlocksPlacedCount { get; set; }
        public User User { get; set; }
    }
}
