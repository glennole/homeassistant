using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Service.Models;
using Serilog;

namespace HomeAssistant.Service;

public class WaterHeater : Switch
{
    private Sensor<HeavyDutySwitchKwhAttributes> AccumulatedKwhSensor { get; set; }

    public HeavyDutySwitch HeavyDutySwitch { get; set; }
    
    public WaterHeater(string entityId, IHomeAssistantProxy homeAssistantProxy) : base(entityId, homeAssistantProxy)
    {
        AccumulatedKwhSensor =
            new Sensor<HeavyDutySwitchKwhAttributes>("sensor.heavy_duty_switch_electric_consumption_kwh",
                homeAssistantProxy);
        HeavyDutySwitch = new HeavyDutySwitch()
        {
            HeavyDutySwitchId = entityId
        };
    }
    
    public State OnWhenWithinOperatingHours(int hour, List<IDailyHourPrice> operatingHours, decimal dailyAveragePrice)
    {
        var currentState = GetState();
        HeavyDutySwitch.State = State;
        HeavyDutySwitch.ReadingAt = DateTime.Now;
        HeavyDutySwitch.StateLastChangedAt = LastChangedAt;

        var reading = AccumulatedKwhSensor.GetReadings();
        decimal.TryParse(reading.State, out decimal accumulatedKwh);
        HeavyDutySwitch.AccumulatedKwh = accumulatedKwh;
        HeavyDutySwitch.AccumulatedKwhLastChangedAt = reading.LastChangedAt;

        if (operatingHours.Any(p => p.Hour == hour))
        {
            Log.Information("Water heater is turned {@state} between {@from} and {@to}. Average:{@averagePrice}. Current: {@currentPrice}.",
                State.On, 
                hour, 
                hour == 23 ? 0 : hour + 1, 
                dailyAveragePrice,
                operatingHours.First(oh => oh.Hour == hour).Price);
            return currentState == State.On ? currentState : TurnOn();
        }
        Log.Information("Water heater is turned {@state} between {@from} and {@to}", State.Off, hour, hour == 23 ? 0 : hour + 1);
        return currentState == State.Off ? currentState : TurnOff();
    }
}