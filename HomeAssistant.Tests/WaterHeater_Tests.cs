using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using HomeAssistant.PostgreSql.Repositories;
using HomeAssistant.Service;
using HomeAssistant.Service.HvaKosterStrommen;
using HomeAssistant.Service.Services;
using Xunit;

namespace HomeAssistant.Tests;

public class WaterHeaterTests
{
    private readonly WaterHeater _waterHeater;
    private readonly NordpoolSensor _sensor;
    private readonly IHeavyDutySwitchRepository _heavyDutySwitchRepository;
    private readonly IDailyHourPriceService _dailyHourPriceService;
    
    public WaterHeaterTests()
    {
        IHomeAssistantProxy homeAssistantProxy = new HomeAssistantProxyMocked();
        //_switch = new Switch("switch.heavy_duty_switch", homeAssistantProxy, 6);
        _sensor = new NordpoolSensor("sensor.nordpool_kwh_krsand_nok_3_095_025", homeAssistantProxy);
        _waterHeater = new WaterHeater("switch.heavy_duty_switch", homeAssistantProxy);
        _dailyHourPriceService = new DailyHourPriceService(new DailyHourPriceRepositoryMocked(), new HvaKosterStrommenHourPriceService());
        _heavyDutySwitchRepository = new HeavyDutySwitchRepository("");
    }

    [Fact]
    public void GetUsage_ShouldReturnANumberAboveZero()
    {
        Assert.Throws<NotImplementedException>(() => _waterHeater.GetUsage());
    }

    [Fact]
    public void GetState_ShouldReturnAStateNotUnkown()
    {
        Assert.True(_waterHeater.GetState() != State.Unknown);
    }

    [Fact]
    public async Task GetConsumptionByDateWhenThereAreNoReadings_ShouldReturnAnArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _heavyDutySwitchRepository.GetDailyConsumptionByDateAsync(new DateTime(2025, 6, 10)));
    }

    [Fact]
    public void DetermineOnOrOffAtFifttheenOClock_ShouldReturnTrue()
    {
        List<IDailyHourPrice> operatingHours =
            (_dailyHourPriceService.GetOptimalHeatingHoursByDate(new DateTime(2024,10,18))).Result.ToList();
        decimal dailyAverageHourPrice = _dailyHourPriceService.GetDailyAverageHourPrice(new DateTime(2024,10,18)).Result;
        
        Assert.True(_waterHeater.OnWhenWithinOperatingHours(15, operatingHours, dailyAverageHourPrice) == State.On);
    }
}