namespace GameAPI.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string email, string confirmationLink);
        Task SendEmailConfirmedNotificationAsync(string email);
    }
}
