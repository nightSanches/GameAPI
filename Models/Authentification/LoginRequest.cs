using GameAPI.Classes;
using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models.Authentification
{
    public class LoginRequest
    {
        [NicknameOrEmail]
        public string NicknameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
