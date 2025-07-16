namespace HomeAssistant.Service.Services;

public interface IEmailService
{
    Task<bool> SendEmail(string subject, string contentPlainText, string contentHtml);
}