namespace GameAPI.Models.Authentification
{
    /// <summary>
    /// Модель ответа на успешную авторизацию
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Никнейм пользователя
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Токен доступа
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Почта пользователя
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Дата регистрации пользователя
        /// </summary>
        public DateTime RegistrationDate { get; set; }

    }
}
