using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models.Authentification
{
    public class RegisterRequest
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._-]{1,30}$", ErrorMessage = "Никнейм может содержать только буквы, цифры, точки, подчёркивания и дефисы (до 30 символов)")]
        public string Nickname { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
