using System.Diagnostics;

namespace goHomeAssistant.Service;

public class ThresholdHandler
{
    public Sensor<NordPoolAttributes> NordpoolSensor { get;  }
    
    public ThresholdHandler(Sensor<NordPoolAttributes> nordpoolSensor)
    {
        NordpoolSensor = nordpoolSensor;
    }

    public bool IsCurrentPriceBelowThreshold(int hour, int numberOfOperationHoursPerDay)
    {
        double selectedHourPrice = NordpoolSensor.Attributes.Today.ToList()[hour];
        var sortedList = NordpoolSensor.Attributes.Today.OrderBy(t => t).ToList();

        return sortedList[numberOfOperationHoursPerDay - 1] >= selectedHourPrice;
    }
}