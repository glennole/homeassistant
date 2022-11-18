using System.Diagnostics;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Service.Models;
using Serilog;

namespace HomeAssistant.Service;

public class WaterHeater : Switch
{
    private Sensor<HeavyDutySwitchKwhAttributes> AccumulatedKwhSensor { get; set; }
    public int MinimumOperatingHoursPerDay { get; }

    public HeavyDutySwitch HeavyDutySwitch { get; set; }
    
    public WaterHeater(string entityId, IHomeAssistantProxy homeAssistantProxy, int minimumOperatingHoursPerDay) : base(entityId, homeAssistantProxy)
    {
        MinimumOperatingHoursPerDay = minimumOperatingHoursPerDay;
        AccumulatedKwhSensor =
            new Sensor<HeavyDutySwitchKwhAttributes>("sensor.heavy_duty_switch_electric_consumption_kwh",
                homeAssistantProxy);
        HeavyDutySwitch = new HeavyDutySwitch()
        {
            HeavyDutySwitchId = entityId
        };
    }

    public State OnWhenBelowAverage(int hour, NordpoolSensor sensor)
    {
        var currentState = GetState();
        HeavyDutySwitch.State = State;
        HeavyDutySwitch.ReadingAt = DateTime.Now;
        HeavyDutySwitch.StateLastChangedAt = LastChangedAt;

        var reading = AccumulatedKwhSensor.GetReadings();
        decimal.TryParse(reading.State, out decimal accumulatedKwh);
        HeavyDutySwitch.AccumulatedKwh = accumulatedKwh;
        HeavyDutySwitch.AccumulatedKwhLastChangedAt = reading.LastChangedAt;

        if (sensor.GetTodaysPeriodesBelowAveragePrice().Any(p => p.Contains(hour)))
        {
            Log.Information("Water heater is turned {@state} between {@from} and {@to}. Average:{@averagePrice}. Current: {@currentPrice}.", State.On, hour, hour == 23 ? 0 : hour + 1, sensor.GetReadings().Attributes.Average, sensor.GetReadings().Attributes.Today[hour]);
            return currentState == State.On ? currentState : TurnOn();
        }
        Log.Information("Water heater is turned {@state} between {@from} and {@to}", State.Off, hour, hour == 23 ? 0 : hour + 1);
        return currentState == State.Off ? currentState : TurnOff();
    }
    
    public bool IsCurrentPriceBelowThreshold(int hour, NordpoolSensor sensor)
    {
        double selectedHourPrice =  sensor.Attributes.Today.ToList()[hour];
        var sortedList = sensor.Attributes.Today.OrderBy(t => t).ToList();

        return sortedList[MinimumOperatingHoursPerDay - 1] >= selectedHourPrice;
    }
}