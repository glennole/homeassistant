using SendGrid;
using SendGrid.Helpers.Mail;

namespace HomeAssistant.Service.SendGrid;

public class EmailService : IEmailService
{
    private SendGridClient _sendGridClient;
    
    public EmailService(IConfiguration configuration)
    {
        var apiKey = configuration["SendGrid:ApiKey"];
        _sendGridClient = new SendGridClient(apiKey);
    }
    
    public async Task<bool> SendEmail()
    {
        var from = new EmailAddress("homeassistant@glennole.com", "HomeAssistant");
        var subject = "Strømforbruk i går";
        var to = new EmailAddress("post@gohaugen.com", "Glenn Ole Haugen");
        var plainTextContent = "Strømforbruk: 75kWh";
        var htmlContent = "<string>Strømforbruk: 75kWh</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        var response = await _sendGridClient.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
            return true;

        return false;
    }
}