using System;
using System.Collections.Generic;
using System.Linq;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.Repositories;
using HomeAssistant.Service.HvaKosterStrommen;
using HomeAssistant.Service.Models;
using HomeAssistant.Service.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HomeAssistant.Tests;

public class DailyHourPriceService_Tests
{
    private readonly IDailyHourPriceService _dailyHourPriceService;

    public DailyHourPriceService_Tests()
    {
        IDailyHourPriceRepository dailyHourPriceRepository = new DailyHourPriceRepositoryMocked();
        _dailyHourPriceService =
            new DailyHourPriceService(dailyHourPriceRepository, new HvaKosterStrommenHourPriceService());
        
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 0, Price = 0.89m, Description = "[0, 1>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 1, Price = 0.79m, Description = "[1, 2>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 2, Price = 0.69m, Description = "[2, 3>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 3, Price = 0.59m, Description = "[3, 4>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 4, Price = 0.49m, Description = "[4, 5>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 5, Price = 0.39m, Description = "[5, 6>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 6, Price = 0.29m, Description = "[6, 7>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 7, Price = -0.09m, Description = "[7, 8>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 8, Price = 0.49m, Description = "[8, 9>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 9, Price = 0.69m, Description = "[9, 10>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 10, Price = 0.79m, Description = "[10, 11>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 11, Price = 0.79m, Description = "[11, 12>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 12, Price = 0.69m, Description = "[12, 13>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 13, Price = 0.69m, Description = "[13, 14>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 14, Price = 0.66m, Description = "[14, 15>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 15, Price = 0.50m, Description = "[15, 16>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 16, Price = 0.52m, Description = "[16, 17>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 17, Price = 0.56m, Description = "[17, 18>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 18, Price = 0.70m, Description = "[18, 19>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 19, Price = 0.80m, Description = "[19, 20>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 20, Price = 0.90m, Description = "[20, 21>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 21, Price = 0.92m, Description = "[21, 22>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 22, Price = 0.92m, Description = "[22, 23>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-2), Hour = 23, Price = 0.92m, Description = "[23, 24>" });
        
        
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 0, Price = 0.803m, Description = "[0, 1>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 1, Price = 0.799m, Description = "[1, 2>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 2, Price = 0.799m, Description = "[2, 3>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 3, Price = 0.799m, Description = "[3, 4>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 4, Price = 0.798m, Description = "[4, 5>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 5, Price = 0.796m, Description = "[5, 6>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 6, Price = 0.792m, Description = "[6, 7>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 7, Price = 0.779m, Description = "[7, 8>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 8, Price = 0.71m, Description = "[8, 9>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 9, Price = 0.567m, Description = "[9, 10>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 10, Price = 0.218m, Description = "[10, 11>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 11, Price = 0.071m, Description = "[11, 12>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 12, Price = 0.003m, Description = "[12, 13>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 13, Price = 0.003m, Description = "[13, 14>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 14, Price = 0.023m, Description = "[14, 15>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 15, Price = 0.209m, Description = "[15, 16>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 16, Price = 0.4m, Description = "[16, 17>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 17, Price = 0.657m, Description = "[17, 18>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 18, Price = 0.784m, Description = "[18, 19>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 19, Price = 0.794m, Description = "[19, 20>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 20, Price = 0.802m, Description = "[20, 21>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 21, Price = 0.794m, Description = "[21, 22>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 22, Price = 0.765m, Description = "[22, 23>" });
        dailyHourPriceRepository.AddAsync(new DailyHourPrice()
            { Date = DateTime.Now.AddDays(-3), Hour = 23, Price = 0.706m, Description = "[23, 24>" });
    }

    [Fact]
    public async void GetMissingDatesOfDailyHourPrices_ShouldReturnOneDayMissing()
    {
        var result = await _dailyHourPriceService.FetchAndStoreMissingDailyHourPricesAsync();
        
        Assert.True(result > 0);
    }

    [Fact]
    public async void FetchAndStoreDailyHoursForTwoDaysAgo_ShouldReturnZero()
    {
        var result = await _dailyHourPriceService.FetchAndStoreDailyHourPricesByDateAsync(DateTime.Now.AddDays(-2));
        
        Assert.True(result == 0);
    }

    [Fact]
    public async void GetMinimumOperationHours_ShouldReturnMinimum8Hours()
    {
        List<IDailyHourPrice> dailyHourPriceRepository =
            (await _dailyHourPriceService.GetOptimalHeatingHoursByDate(new DateTime(2024, 10, 18))).ToList();
        
        Assert.True(dailyHourPriceRepository.Count > 7);
    }
}