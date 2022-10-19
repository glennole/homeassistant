using System.Linq;
using goHomeAssistant.Service;
using Xunit;

namespace goHomeAssistant.Tests;

public class Sensor_Tests
{
    private readonly Sensor<NordPoolAttributes> _sensor;

    public Sensor_Tests()
    {
        IHomeAssistantProxy homeAssistantProxy = new HomeAssistantProxyMocked();
        
        _sensor = new Sensor<NordPoolAttributes>("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
    }

    [Fact]
    public void GetSensorReadings_ReturnsReadings()
    {
        Assert.True(_sensor.GetReadings() != null);
    }

    [Fact]
    public void GetNordPoolReadings_ReturnsCurrentPrice()
    {
        Assert.True(_sensor.GetReadings().CurrentPrice > 0.0);
    }

    [Fact]
    public void GetNordPoolReadings_TodayPrices()
    {
        Assert.True(_sensor.GetReadings().Today.Count() == 24);
    }
}
