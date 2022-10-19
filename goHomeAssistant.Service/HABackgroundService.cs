using Quartz;
using Quartz.Impl;

namespace goHomeAssistant.Service;

public class VVSBackgroundService : BackgroundService
{
    private readonly ILogger<VVSBackgroundService> _logger;
    private readonly List<Switch> _switches;
    private readonly IHomeAssistantProxy _homeAssistantProxy;

    public List<Switch> Switches => _switches;
    public Sensor<NordPoolAttributes> NordpoolSensor { get; }

    public ThresholdHandler ThresholdHandler { get; }

    public VVSBackgroundService(ILogger<VVSBackgroundService> logger, IHomeAssistantProxy homeAssistantProxy)
    {
        _logger = logger;
        
        
        _switches = new List<Switch>();
        
        // _switches.Add(new Switch("switch.heavy_duty_switch", homeAssistantProxy, 5));
        // NordpoolSensor = new Sensor<NordPoolAttributes>("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
        // ThresholdHandler = new ThresholdHandler(NordpoolSensor);
        //
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

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