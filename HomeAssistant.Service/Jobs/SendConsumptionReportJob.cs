using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Service.Services;
using Quartz;
using Serilog;

namespace HomeAssistant.Service.Jobs;

public class SendConsumptionReportJob  : IJob
{
    private readonly IEmailService _emailService;
    private readonly IHeavyDutySwitchRepository _heavyDutySwitchRepository;
    
    public SendConsumptionReportJob(IEmailService emailService, IHeavyDutySwitchRepository heavyDutySwitchRepository)
    {
        Log.Debug("Send consumption report job initiated");
        _emailService = emailService;
        _heavyDutySwitchRepository = heavyDutySwitchRepository;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        Log.Debug("Sending consumption report...");
        try
        {
            var dailyConsumption =
                await _heavyDutySwitchRepository.GetDailyConsumptionByDateAsync(DateTime.Today.AddDays(-1));
            string contentPlain = $"Strømforbruk: {dailyConsumption.Consumption}. Kostnad: {dailyConsumption.Cost}";
            string contentHtml =
                $"<strong>Strømforbruk:</strong> {dailyConsumption.Consumption}. <strong>Kostnad:</strong> {dailyConsumption.Cost}.";
            await _emailService.SendEmail($"Rapport for VVB {dailyConsumption.CalculationDate}", contentPlain,
                contentHtml);
            Log.Debug("Consumption report sent successfully.");
        }
        catch (ArgumentException ae)
        {
            Log.Warning(ae.Message);
        }
        catch (Exception e)
        {
            Log.Error(e, "EmailService.SendEmail() failed");
        }
    }
}