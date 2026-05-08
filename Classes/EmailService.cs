using GameAPI.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GameAPI.Classes
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

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

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
    }
}
