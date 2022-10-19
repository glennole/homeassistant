using Quartz;

namespace goHomeAssistant.Service;

public class WaterHeaterJob : IJob
{
    private readonly IHomeAssistantProxy _homeAssistantProxy;
    private readonly Switch _switch;
    
    public WaterHeaterJob(IHomeAssistantProxy homeAssistantProxy)
    {
        _homeAssistantProxy = homeAssistantProxy;
        _switch = new Switch("switch.heavy_duty_switch", homeAssistantProxy, 5);
    }
    
    public async Task Execute(IJobExecutionContext context)
    {

        _switch.TurnOn();
        Console.WriteLine("Water heater job " + _switch.MinimumOperatingHoursPerDay);
    }
}