using HomeAssistant.Service.SendGrid;
using Quartz;
using Serilog;

namespace HomeAssistant.Service;

public class SendConsumptionReportJob  : IJob
{
    private readonly IEmailService _emailService;
    
    public SendConsumptionReportJob(IEmailService emailService)
    {
        Log.Debug("Send consumption report job initiated");
        _emailService = emailService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        Log.Debug("Sending consumption report...");
        try
        {
            await _emailService.SendEmail();
            Log.Debug("Consumption report sent successfully.");
        }
        catch (Exception e)
        {
            Log.Error(e, "EmailService.SendEmail() failed");
        }
    }
}