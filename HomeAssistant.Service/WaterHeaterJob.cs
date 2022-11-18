using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using Quartz;
using Serilog;
using ILogger = Serilog.ILogger;

namespace HomeAssistant.Service;

public class WaterHeaterJob : IJob
{
    private readonly IHomeAssistantProxy _homeAssistantProxy;
    private readonly IHeavyDutySwitchRepository _heavyDutySwitchRepository;
    private readonly NordpoolSensor _sensor;
    private readonly WaterHeater _waterHeater;

    public WaterHeaterJob(IHomeAssistantProxy homeAssistantProxy, IHeavyDutySwitchRepository heavyDutySwitchRepository)
    {
        Log.Debug("Water heater job initiated: {0}", homeAssistantProxy);
        _homeAssistantProxy = homeAssistantProxy;
        _heavyDutySwitchRepository = heavyDutySwitchRepository;
        _waterHeater = new WaterHeater("switch.heavy_duty_switch", homeAssistantProxy, 5);
        _sensor = new NordpoolSensor("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
        
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        Log.Debug("Turn on water heater when price is below average");
        var result = _waterHeater.OnWhenBelowAverage(DateTime.Now.Hour, _sensor);
        Log.Debug($"Water heater turned {result.ToString()} at {DateTime.Now}.");
        
        Log.Debug("Persisting current state of water heater");
        _heavyDutySwitchRepository.AddAsync(_waterHeater.HeavyDutySwitch.MapToDto());
        Log.Debug("Persist current state of water heater finished successfully");
    }

}