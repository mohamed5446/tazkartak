namespace Tazkartk.Email
{
    public interface IEmailService
    {
        Task SendEmail(EmailRequest emailRequest);
    }
}
