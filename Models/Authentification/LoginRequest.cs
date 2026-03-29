using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GameAPI.Classes;

namespace GameAPI.Models.Authentification
{
    /// <summary>
    /// Модель запроса для авторизации пользователя
    /// </summary>
    public class LoginRequest
    {
        [NicknameOrEmail]
        public string NicknameOrEmail { get; set; }

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [StringLength(200, ErrorMessage = "Пароль не может превышать 200 символов")]
        public string Password { get; set; }
    }
}
