using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Database;
using HomeAssistant.Service.Configuration;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Serilog;

namespace HomeAssistant.Service;

public class VVSBackgroundService : BackgroundService
{
    private readonly List<Switch> _switches;
    private readonly IHomeAssistantProxy _homeAssistantProxy;

    public List<Switch> Switches => _switches;
    public Sensor<NordPoolAttributes> NordpoolSensor { get; }

    public WaterHeater WaterHeater { get; }

    public VVSBackgroundService(IOptions<PostgresqlOptions> postgresqlConfig)
    {
        _switches = new List<Switch>();
        
        DatabaseWorker.Migrate(postgresqlConfig.Value.ConnectionString);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Debug("Worker running at: {time}", DateTimeOffset.Now);

            // foreach (var @switch in _switches)
            // {
            //     if (ThresholdHandler.IsCurrentPriceBelowThreshold(23, 5))
            //     {
            //         Console.WriteLine("ON");
            //         //@switch.TurnOn();
            //     }
            //     else
            //     {
            //         Console.WriteLine("OFF");
            //         //@switch.TurnOff();
            //     }
            // }
            
            await Task.Delay(5000, stoppingToken);
        }
    }
}