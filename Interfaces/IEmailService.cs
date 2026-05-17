namespace GameAPI.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для отправки email-уведомлений.
    /// Реализует отправку писем подтверждения регистрации и уведомлений о подтверждении email.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Отправляет письмо с ссылкой для подтверждения email адреса при регистрации.
        /// </summary>
        /// <param name="email">Адрес электронной почты получателя</param>
        /// <param name="confirmationLink">Ссылка для подтверждения email (содержит токен)</param>
        /// <returns>Задача асинхронной операции отправки письма</returns>
        Task SendConfirmationEmailAsync(string email, string confirmationLink);
        
        /// <summary>
        /// Отправляет уведомление об успешном подтверждении email адреса.
        /// </summary>
        /// <param name="email">Адрес электронной почты получателя</param>
        /// <returns>Задача асинхронной операции отправки письма</returns>
        Task SendEmailConfirmedNotificationAsync(string email);
    }
}
