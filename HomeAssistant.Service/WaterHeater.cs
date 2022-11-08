using System.Diagnostics;
using Serilog;

namespace HomeAssistant.Service;

public class WaterHeater : Switch
{
    public int MinimumOperatingHoursPerDay { get; }

    public WaterHeater(string entityId, IHomeAssistantProxy homeAssistantProxy, int minimumOperatingHoursPerDay) : base(entityId, homeAssistantProxy)
    {
        MinimumOperatingHoursPerDay = minimumOperatingHoursPerDay;
    }

    public State OnWhenBelowAverage(int hour, NordpoolSensor sensor)
    {
        var currentState = GetState();
        
        if (sensor.GetTodaysPeriodesBelowAveragePrice().Any(p => p.Contains(hour)))
        {
            Log.Information("Water heater is turned {@state} between {@from} and {@to}. Average:{@averagePrice}. Current: {@currentPrice}.", State.On, hour, hour == 23 ? 0 : hour + 1, sensor.GetReadings().Average, sensor.GetReadings().Today[hour]);
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