

using Smtp2Go.Api;
using Smtp2Go.Api.Models.Emails;

namespace HomeAssistant.Service.Services;

public class EmailService : IEmailService
{
    private Smtp2GoApiService _smtp2GoApiService;
    
    public EmailService(IConfiguration configuration)
    {
        var apiKey = configuration["Smtp2Go:ApiKey"];
        _smtp2GoApiService = new Smtp2GoApiService(apiKey);
    }
    
    public async Task<bool> SendEmail(string subject, string plainTextContent, string htmlContent)
    {
        var message = new EmailMessage("HomeAssistant <homeassistant@glennole.com>",
            "Glenn Ole Haugen <post@gohaugen.com>");
        message.Subject = subject;
        message.BodyText = plainTextContent;
        message.BodyHtml = htmlContent;
        
        var response = await _smtp2GoApiService.SendEmail(message);
        if (response.Data.Succeeded == 1)
            return true;

        return false;
    }
}