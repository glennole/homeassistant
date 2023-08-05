using System;
using System.Linq;
using HomeAssistant.Service.HvaKosterStrommen;
using Xunit;

namespace HomeAssistant.Tests;

public class HvaKosterStrommen_Tests
{
    private readonly IHvaKosterStrommenHourPriceService _hvaKosterStrommenHourPriceService;

    public HvaKosterStrommen_Tests()
    {
        _hvaKosterStrommenHourPriceService = new HvaKosterStrommenHourPriceService();
    }
    
    [Fact]
    public async void GetTodaysDailyHourPrices()
    {
        var result = await _hvaKosterStrommenHourPriceService.GetHourPricesByDate(DateTime.Now);
        Assert.True(result.Count() == 24);
    }
}