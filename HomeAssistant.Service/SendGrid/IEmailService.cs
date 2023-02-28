namespace HomeAssistant.Service.SendGrid;

public interface IEmailService
{
    Task<bool> SendEmail(string subject, string contentPlainText, string contentHtml);
}