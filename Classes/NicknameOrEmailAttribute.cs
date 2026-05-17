using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GameAPI.Classes
{
    /// <summary>
    /// Атрибут валидации для проверки ввода пользователя.
    /// Принимает либо корректный email, либо никнейм (до 30 символов, буквы/цифры/._-).
    /// Используется для полей входа, где пользователь может ввести либо никнейм, либо email.
    /// </summary>
    public class NicknameOrEmailAttribute : ValidationAttribute
    {
        // Regex-паттерн для проверки email адреса
        private const string EmailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+$";
        
        // Regex-паттерн для проверки никнейма: 1-30 символов, буквы, цифры, точка, подчёркивание, дефис
        private const string NicknamePattern = @"^[a-zA-Z0-9._-]{1,30}$";

        /// <summary>
        /// Проверка значения на соответствие формату email или никнейма.
        /// </summary>
        /// <param name="value">Значение для валидации (объект)</param>
        /// <param name="validationContext">Контекст валидации</param>
        /// <returns>ValidationResult.Success если значение корректно, иначе сообщение об ошибке</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Пустое или null значение считается невалидным
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult("Имя пользователя обязательно для заполнения.");

            string input = value.ToString();

            // Проверяем, является ли ввод email адресом
            if (Regex.IsMatch(input, EmailPattern))
                return ValidationResult.Success;

            // Проверяем, является ли ввод корректным никнеймом
            if (Regex.IsMatch(input, NicknamePattern))
                return ValidationResult.Success;

            // Если ни одно условие не выполнено, возвращаем ошибку
            return new ValidationResult("Введите корректный никнейм (до 30 символов, буквы/цифры/._-) или email.");
        }
    }
}
