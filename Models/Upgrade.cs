using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    [Table("Upgrades")]
    public class Upgrade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
