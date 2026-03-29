using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GameAPI.Classes
{
    public class NicknameOrEmailAttribute : ValidationAttribute
    {
        private const string EmailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+$";
        private const string NicknamePattern = @"^[a-zA-Z0-9._-]{1,30}$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult("Имя пользователя обязательно для заполнения.");

            string input = value.ToString();

            if (Regex.IsMatch(input, EmailPattern))
                return ValidationResult.Success;

            if (Regex.IsMatch(input, NicknamePattern))
                return ValidationResult.Success;

            return new ValidationResult("Введите корректный никнейм (до 30 символов, буквы/цифры/._-) или email.");
        }
    }
}
