using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Districts")]
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column("Unlock_Rep_Req")]
        public int UnlockRepReq { get; set; }
        [Column("Difficulty_Multiplier")]
        public decimal DifficultyMultiplier { get; set; }
        [Column("Sort_Order")]
        public int SortOrder { get; set; }
    }
}
