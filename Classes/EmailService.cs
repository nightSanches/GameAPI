using GameAPI.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GameAPI.Classes
{
    /// <summary>
    /// Сервис для отправки email-уведомлений через SMTP.
    /// Реализует интерфейс IEmailService и использует настройки из конфигурации приложения.
    /// Отправляет письма подтверждения регистрации и уведомления об успешном подтверждении.
    /// </summary>
    public class EmailService : IEmailService
    {
        // Настройки SMTP сервера для отправки писем
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Конструктор сервиса email. Получает настройки через dependency injection.
        /// </summary>
        /// <param name="emailSettings">Настройки SMTP, полученные из appsettings.json</param>
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Отправляет HTML-письмо с кнопкой подтверждения email адреса.
        /// Использует SMTP клиент для подключения к почтовому серверу и отправки сообщения.
        /// </summary>
        /// <param name="email">Адрес электронной почты получателя</param>
        /// <param name="confirmationLink">Ссылка для подтверждения (содержит уникальный токен)</param>
        /// <returns>Задача асинхронной операции отправки</returns>
        public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Apex Town", _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Подтверждение email";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8'>
        <title>Подтверждение email</title>
    </head>
    <body style='font-family: Arial, sans-serif; text-align: center;'>
        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h1 style='color: #4CAF50;'>Подтверждение email</h1>
            <p>Для завершения регистрации нажмите на кнопку ниже:</p>
            <a href='{confirmationLink}' 
               style='display: inline-block; 
                      background-color: #4CAF50; 
                      color: white; 
                      padding: 12px 24px; 
                      text-decoration: none; 
                      border-radius: 5px; 
                      font-size: 16px;
                      margin: 20px 0;'>
                Подтвердить email
            </a>
            <p style='color: #888; font-size: 12px;'>Если кнопка не работает, скопируйте ссылку в браузер:<br>
            <a href='{confirmationLink}' style='color: #4CAF50;'>{confirmationLink}</a></p>
            <p>Ссылка действительна 24 часа.</p>
            <p style='color: #888; font-size: 12px;'>Если это не вы, проигнорируйте это письмо.</p>
        </div>
    </body>
    </html>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        
        /// <summary>
        /// Отправляет HTML-уведомление об успешном подтверждении email адреса.
        /// Используется после того, как пользователь перешёл по ссылке подтверждения.
        /// </summary>
        /// <param name="email">Адрес электронной почты получателя</param>
        /// <returns>Задача асинхронной операции отправки</returns>
        public async Task SendEmailConfirmedNotificationAsync(string email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Apex Town", _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Email подтверждён";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
                    h1 {{ color: #4CAF50; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>✅ Email подтверждён</h1>
                    <p>Здравствуйте!</p>
                    <p>Ваш email был успешно подтверждён. Не забудьте забрать свой подарок в главном меню игры!</p>
                    <p>Спасибо, что играете в нашу игру!</p>
                    <hr>
                    <p style='color: #888; font-size: 12px;'>Если вы не выполняли это действие, пожалуйста, свяжитесь с поддержкой.</p>
                </div>
            </body>
            </html>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    /// <summary>
    /// Класс настроек для SMTP сервера отправки email.
    /// Заполняется из секции "EmailSettings" в appsettings.json.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Адрес SMTP сервера (например, smtp.gmail.com)
        /// </summary>
        public string SmtpServer { get; set; }
        
        /// <summary>
        /// Порт SMTP сервера (обычно 587 для TLS)
        /// </summary>
        public int SmtpPort { get; set; }
        
        /// <summary>
        /// Email адрес отправителя
        /// </summary>
        public string SenderEmail { get; set; }
        
        /// <summary>
        /// Пароль или app-specific password для отправителя
        /// </summary>
        public string SenderPassword { get; set; }
    }
}
