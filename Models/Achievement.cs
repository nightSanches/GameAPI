using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Achievements")]
    public class Achievement
    {
        public int Id { get; set; }
        [Column("District_Id")]
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column("Condition_Type")]
        public string ConditionType { get; set; }
        [Column("Condition_Value")]
        public int ConditionValue { get; set; }
        [Column("Reward_Rep")]
        public int RewardRep { get; set; }
    }
}
