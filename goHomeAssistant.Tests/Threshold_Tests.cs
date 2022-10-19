using goHomeAssistant.Service;
using Xunit;

namespace goHomeAssistant.Tests;

public class ThresholdTests
{
    private readonly Switch _switch;
    private readonly ThresholdHandler _thresholdHandler;
    
    public ThresholdTests()
    {
        IHomeAssistantProxy homeAssistantProxy = new HomeAssistantProxyMocked();
        _switch = new Switch("switch.heavy_duty_switch", homeAssistantProxy, 6);
        var sensor = new Sensor<NordPoolAttributes>("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
        _thresholdHandler = new ThresholdHandler(sensor);
    }

    [Fact]
    public void CurrentPriceIsBelowThreshold_ReturnsTrue()
    {
        Assert.True(_thresholdHandler.IsCurrentPriceBelowThreshold(0, 6));
    }
    
    [Fact]
    public void CurrentPriceIsMax_ShouldReturnFalse()
    {
        Assert.True(!_thresholdHandler.IsCurrentPriceBelowThreshold(7, _switch.MinimumOperatingHoursPerDay) && _thresholdHandler.NordpoolSensor.Attributes.CurrentPrice == _thresholdHandler.NordpoolSensor.Attributes.Max);
    }
}