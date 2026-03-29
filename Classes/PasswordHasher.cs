using BCrypt.Net;

namespace GameAPI.Classes
{
    public class PasswordHasher
    {
        /// <summary>
        /// Хэширует пароль с помощью BCrypt
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Проверяет соответствие пароля хэшу
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
