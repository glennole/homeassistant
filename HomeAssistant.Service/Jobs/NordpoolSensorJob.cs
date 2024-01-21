using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Service.Models;
using Quartz;
using Serilog;

namespace HomeAssistant.Service.Jobs;

public class NordpoolSensorJob : IJob
{
    private readonly IHomeAssistantProxy _homeAssistantProxy;
    private readonly IDailyHourPriceRepository _dailyHourPriceRepository;
    private readonly NordpoolSensor _sensor;


    public NordpoolSensorJob(IDailyHourPriceRepository dailyHourPriceRepository, IHomeAssistantProxy homeAssistantProxy)
    {
        _dailyHourPriceRepository = dailyHourPriceRepository;
        _sensor = new NordpoolSensor("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            if (!await _dailyHourPriceRepository.HasPricesForGivenDate(DateTime.Now))
            {
                for (int i = 0; i < 24; i++)
                {
                    var dailyHourPrice = new DailyHourPrice()
                    {
                        Date = DateTime.Now,
                        Description = $"[{i}, {i + 1}>",
                        Hour = i,
                        Price = (decimal) _sensor.Attributes.Today[i]
                    };

                    await _dailyHourPriceRepository.AddAsync(dailyHourPrice);
                }
            }

            if (!await _dailyHourPriceRepository.HasPricesForGivenDate(DateTime.Now.AddDays(1)) &&
                _sensor.Attributes.Tomorrow.Any())
            {
                for (int i = 0; i < 24; i++)
                {
                    var dailyHourPrice = new DailyHourPrice()
                    {
                        Date = DateTime.Now.AddDays(1),
                        Description = $"[{i}, {i + 1}>",
                        Hour = i,
                        Price = (decimal) _sensor.Attributes.Tomorrow[i]
                    };

                    _dailyHourPriceRepository.AddAsync(dailyHourPrice);
                }
            }
        }
        catch (Exception e)
        {
            //Required to not let an error stop future executions of the job.
            Log.Error(e, "Nordpool sensor job failed!");
        }
    }
}