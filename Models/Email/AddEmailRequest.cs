using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models.Email
{
    public class AddEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
