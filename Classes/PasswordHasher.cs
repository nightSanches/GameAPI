using BCrypt.Net;

namespace GameAPI.Classes
{
    /// <summary>
    /// Утилита для хеширования и проверки паролей с использованием алгоритма BCrypt.
    /// BCrypt обеспечивает безопасное хранение паролей с добавлением соли и адаптивной сложностью.
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        /// Создаёт хеш пароля с помощью алгоритма BCrypt.
        /// Автоматически генерирует соль и использует адаптивную стоимость вычислений.
        /// </summary>
        /// <param name="password">Пароль в открытом виде для хеширования</param>
        /// <returns>Хеш пароля в формате BCrypt</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Проверяет соответствие пароля его хешу.
        /// Сравнивает открытый пароль с хешем, используя встроенную в хеш соль.
        /// </summary>
        /// <param name="password">Пароль в открытом виде для проверки</param>
        /// <param name="hash">Хеш пароля, сохранённый в базе данных</param>
        /// <returns>True если пароль совпадает с хешем, иначе False</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
