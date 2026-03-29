using System.ComponentModel.DataAnnotations;

namespace GameAPI.Models
{
    public class RegisterRequest
    {
        /// <summary>
        /// Никнейм пользователя (обязательный, уникальный)
        /// </summary>
        [Required(ErrorMessage = "Никнейм обязателен для заполнения.")]
        [RegularExpression(@"^[a-zA-Z0-9._-]{1,30}$",
            ErrorMessage = "Никнейм может содержать только буквы, цифры, точки, подчёркивания и дефисы. Длина от 1 до 30 символов.")]
        public string Nickname { get; set; }

        /// <summary>
        /// Email пользователя (необязательный, уникальный, если указан)
        /// </summary>
        [EmailAddress(ErrorMessage = "Неверный формат email.")]
        [StringLength(100, ErrorMessage = "Email не может превышать 100 символов.")]
        public string? Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 200 символов.")]
        public string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Required(ErrorMessage = "Подтверждение пароля обязательно.")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
