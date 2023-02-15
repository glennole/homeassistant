namespace HomeAssistant.Service.SendGrid;

public interface IEmailService
{
    Task<bool> SendEmail();
}