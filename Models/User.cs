using GameAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    /// <summary>
    /// Модель пользователя приложения Apex Town.
    /// Представляет таблицу Users в базе данных.
    /// Содержит данные аутентификации, профиль и навигационные свойства к связанным данным.
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя (первичный ключ)
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Никнейм пользователя (уникальный, до 30 символов)
        /// </summary>
        public string Nickname { get; set; }
        
        /// <summary>
        /// Хеш пароля пользователя (BCrypt)
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Роль пользователя (например, "player", "admin")
        /// </summary>
        public string Role { get; set; }
        
        /// <summary>
        /// Токен сессии для авторизации API запросов
        /// </summary>
        public string? Token { get; set; }
        
        /// <summary>
        /// Email адрес пользователя (необязательный)
        /// </summary>
        public string? Email { get; set; }
        
        /// <summary>
        /// Дата и время регистрации пользователя
        /// </summary>
        [Column("Registration_date")]
        public DateTime? RegistrationDate { get; set; }
        
        /// <summary>
        /// Флаг подтверждения email адреса
        /// </summary>
        [Column("Email_confirmed")]
        public bool EmailConfirmed { get; set; }
        
        /// <summary>
        /// Токен для подтверждения email (генерируется при регистрации)
        /// </summary>
        [Column("Email_confirmation_token")]
        public string? EmailConfirmationToken { get; set; }
        
        /// <summary>
        /// Время истечения срока действия токена подтверждения email
        /// </summary>
        [Column("Email_confirmation_token_expires")]
        public DateTime? EmailConfirmationTokenExpires { get; set; }

        // Навигационные свойства для связей с другими таблицами
        
        /// <summary>
        /// Кошелёк пользователя (монеты и репутация) - связь один к одному
        /// </summary>
        public UserWallet Wallet { get; set; }
        
        /// <summary>
        /// Коллекция рекордов пользователя по районам - связь один ко многим
        /// </summary>
        public ICollection<UserScore> Scores { get; set; }
        
        /// <summary>
        /// Подарок пользователя (ежедневный бонус) - связь один к одному
        /// </summary>
        public UserGift Gift { get; set; }
        
        /// <summary>
        /// Коллекция бонусов пользователя - связь один ко многим
        /// </summary>
        public ICollection<UserBonus> Bonuses { get; set; }
        
        /// <summary>
        /// Коллекция улучшений пользователя - связь один ко многим
        /// </summary>
        public ICollection<UserUpgrade> Upgrades { get; set; }
    }
}
