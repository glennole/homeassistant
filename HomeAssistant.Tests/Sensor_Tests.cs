using System.Linq;
using HomeAssistant.Service;
using Xunit;

namespace HomeAssistant.Tests;

public class Sensor_Tests
{
    private readonly NordpoolSensor _sensor;

    public Sensor_Tests()
    {
        IHomeAssistantProxy homeAssistantProxy = new HomeAssistantProxyMocked();
        
        _sensor = new NordpoolSensor("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
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
    
    [Fact]
    public void GetAllPeakHours_ReturnsSixPeakHours()
    {
        Assert.True(_sensor.GetPeakHours().Count() == 6);
    }
    
    [Fact]
    public void GetAllBottomHours_ReturnsSixBottomHours()
    {
        
        Assert.True(_sensor.GetBottomHours().Count() == 6);
    }
    
    [Fact]
    public void GetTodaysPeriodesAboveAveragePrice_ReturnsTwoSetsOfHours_FirstSetHavingAFourHoursPeriod()
    {
        Assert.True(_sensor.GetTodaysPeriodesAboveAveragePrice().Count() == 3);
        Assert.True(_sensor.GetTodaysPeriodesAboveAveragePrice().First().Count() == 2);
    }
    
    [Fact]
    public void GetTodaysPeriodesBelowAveragePrice_ReturnsSixSetsOfHours_FirstSetHavingATwoHoursPeriode()
    {
        Assert.True(_sensor.GetTodaysPeriodesBelowAveragePrice().Count() == 6);
        Assert.True(_sensor.GetTodaysPeriodesBelowAveragePrice().First().Count() == 2);
    }
}
