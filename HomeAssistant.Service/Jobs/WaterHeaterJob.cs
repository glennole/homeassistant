using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using HomeAssistant.Service.Services;
using Quartz;
using Serilog;
using ILogger = Serilog.ILogger;

namespace HomeAssistant.Service.Jobs;

public class WaterHeaterJob : IJob
{
    private readonly IHeavyDutySwitchRepository _heavyDutySwitchRepository;
    private readonly WaterHeater _waterHeater;
    private readonly IDailyHourPriceService _dailyHourPriceService;

    public WaterHeaterJob(IHomeAssistantProxy homeAssistantProxy, IHeavyDutySwitchRepository heavyDutySwitchRepository,
        IDailyHourPriceService dailyHourPriceService)
    {
        Log.Debug("Water heater job initiated: {0}", homeAssistantProxy);
        _heavyDutySwitchRepository = heavyDutySwitchRepository;
        _dailyHourPriceService = dailyHourPriceService;
        _waterHeater = new WaterHeater("switch.heavy_duty_switch", homeAssistantProxy);
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            Log.Debug("Turn on water heater when price is below average");

            DateTime osloTz = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Europe/Oslo");

            List<IDailyHourPrice> operatingHours =
                (await _dailyHourPriceService.GetOptimalHeatingHoursByDate(osloTz)).ToList();
            decimal dailyAverageHourPrice = await _dailyHourPriceService.GetDailyAverageHourPrice(osloTz);
            var result = _waterHeater.OnWhenWithinOperatingHours(osloTz.Hour, operatingHours, dailyAverageHourPrice);

            Log.Debug($"Water heater turned {result.ToString()} at {DateTime.Now}.");

            Log.Debug("Persisting current state of water heater");
            await _heavyDutySwitchRepository.AddAsync(_waterHeater.HeavyDutySwitch.MapToDto());
            Log.Debug("Persist current state of water heater finished successfully");
        }
        catch (Exception e)
        {
            //Required to not let an error stop future executions of the job.
            Log.Error(e, "Water heater job failed!");
        }
    }
}