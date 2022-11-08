using Quartz;
using Serilog;
using ILogger = Serilog.ILogger;

namespace HomeAssistant.Service;

public class WaterHeaterJob : IJob
{
    private readonly IHomeAssistantProxy _homeAssistantProxy;
    private readonly NordpoolSensor _sensor;
    private readonly WaterHeater _waterHeater;

    public WaterHeaterJob(IHomeAssistantProxy homeAssistantProxy)
    {
        Log.Debug("Water heater job initiated: {0}", homeAssistantProxy);
        _homeAssistantProxy = homeAssistantProxy;
        _waterHeater = new WaterHeater("switch.heavy_duty_switch", homeAssistantProxy, 5);
        _sensor = new NordpoolSensor("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
        
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        Log.Debug("Turn on water heater when price is below average");
        var result = _waterHeater.OnWhenBelowAverage(DateTime.Now.Hour, _sensor);
        Log.Debug($"Water heater turned {result.ToString()} at {DateTime.Now}.");
    }
}