using System;
using System.Threading.Tasks;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.Repositories;
using HomeAssistant.Service;
using HomeAssistant.Service.HvaKosterStrommen;
using HomeAssistant.Service.Services;
using Xunit;

namespace HomeAssistant.Tests;

public class WaterHeaterServiceTests
{
    private readonly IWaterHeaterService _waterHeaterService;
  //  private readonly IDailyHourPriceService _dailyHourPriceService;
    
    public WaterHeaterServiceTests()
    {
        IDailyHourPriceRepository dailyHourPriceRepository = new DailyHourPriceRepositoryMocked();
        
        _waterHeaterService = new WaterHeaterService(
            new HeavyDutySwitchRepository(""), 
            new DailyHourPriceService(
                new DailyHourPriceRepositoryMocked(), 
                new HvaKosterStrommenHourPriceService(null)
                )
            );
    }

    [Fact]
    public void GetOperationHoursForToday_ShouldReturnAtleast5Hours()
    {
        Assert.True( _waterHeaterService.GetOperatingHoursByDate(new DateTime(2024,10,18)).Result.Count >= 5);
    }
    
   
}