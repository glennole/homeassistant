using System.Runtime.InteropServices.ComTypes;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Service.HvaKosterStrommen;

namespace HomeAssistant.Service.Services;

public interface IDailyHourPriceService
{
    /// <summary>
    /// Finds and adds missing days with hour prices
    /// </summary>
    /// <returns>Number of missing days added</returns>
    Task<int> FetchAndStoreMissingDailyHourPricesAsync();

    Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesByDateAsync(DateTime date);
    Task<int> FetchAndStoreDailyHourPricesByDateAsync(DateTime date);
    Task<IEnumerable<IDailyHourPrice>> GetOptimalHeatingHoursByDate(DateTime date);
    Task<decimal> GetDailyAverageHourPrice(DateTime date);
}

public class DailyHourPriceService : IDailyHourPriceService
{
    private readonly IDailyHourPriceRepository _dailyHourPriceRepository;
    private readonly IHvaKosterStrommenHourPriceService _hvaKosterStrommenHourPriceService;
    private const decimal AlwaysRunWhenPriceBelow = 0.20m;
    private const int MinimumOperatingHours = 6;


    public DailyHourPriceService(IDailyHourPriceRepository dailyHourPriceRepository,
        IHvaKosterStrommenHourPriceService hvaKosterStrommenHourPriceService)
    {
        _dailyHourPriceRepository = dailyHourPriceRepository;
        _hvaKosterStrommenHourPriceService = hvaKosterStrommenHourPriceService;
    }

    public async Task<int> FetchAndStoreMissingDailyHourPricesAsync()
    {
        DateTime lastDailyHourDate = await _dailyHourPriceRepository.GetLastDailyHourDate();

        if ((lastDailyHourDate.Date == DateTime.Now.Date && DateTime.Now.Hour < 13)
            || (lastDailyHourDate.Date > DateTime.Now.Date))
            return 0;

        int counter = 0;
        while (lastDailyHourDate.Date < DateTime.Now.Date)
        {
            IEnumerable<IDailyHourPrice> dailyHourPrices =
                await GetDailyHourPricesFromHvaKosterStrommenByDate(lastDailyHourDate.AddDays(1));
            foreach (IDailyHourPrice dailyHourPrice in dailyHourPrices)
            {
                await _dailyHourPriceRepository.AddAsync(dailyHourPrice);
            }

            lastDailyHourDate = lastDailyHourDate.AddDays(1);
            counter++;
        }

        if (lastDailyHourDate.Date == DateTime.Now.Date && DateTime.Now.Hour > 13)
        {
            IEnumerable<IDailyHourPrice> dailyHourPrices =
                await GetDailyHourPricesFromHvaKosterStrommenByDate(lastDailyHourDate.AddDays(1));
            foreach (IDailyHourPrice dailyHourPrice in dailyHourPrices)
            {
                await _dailyHourPriceRepository.AddAsync(dailyHourPrice);
            }

            counter++;
        }

        return counter;
    }

    public async Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesByDateAsync(DateTime date)
    {
        DateTime dateOslo = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, "Europe/Oslo");
        return AdjustPricesAccordingToGovernmentSubsidies(
            await _dailyHourPriceRepository.GetDailyHourPricesByDate(dateOslo));
    }

    public async Task<int> FetchAndStoreDailyHourPricesByDateAsync(DateTime date)
    {
        IEnumerable<IDailyHourPrice> existingDailyHourPrices =
            await _dailyHourPriceRepository.GetDailyHourPricesByDate(date);
        if (existingDailyHourPrices.Any())
            return 0;

        int counter = 0;
        IEnumerable<IDailyHourPrice> dailyHourPrices = await GetDailyHourPricesFromHvaKosterStrommenByDate(date);
        foreach (IDailyHourPrice dailyHourPrice in dailyHourPrices)
        {
            await _dailyHourPriceRepository.AddAsync(dailyHourPrice);
            counter++;
        }

        return counter;
    }

    public async Task<IEnumerable<IDailyHourPrice>> GetOptimalHeatingHoursByDate(DateTime date)
    {
        List<IDailyHourPrice> dailyHourPrices = (await GetDailyHourPricesByDateAsync(date)).ToList();
        List<IDailyHourPrice> operatingHours = new List<IDailyHourPrice>();
        
        operatingHours.AddRange(dailyHourPrices.Where(dhp => dhp.Price < AlwaysRunWhenPriceBelow));
        operatingHours.AddRange(GetPeriodsBelowAveragePrice(dailyHourPrices).Where(dhp => !operatingHours.Contains(dhp)).OrderBy(dhp => dhp.Price).Take(MinimumOperatingHours));

        if (dailyHourPrices.Count() < MinimumOperatingHours)
        {
            operatingHours.AddRange(GetPeriodsAboveAveragePrice(dailyHourPrices).Where(dhp => !operatingHours.Contains(dhp)).OrderBy(dhp => dhp.Price).Take(MinimumOperatingHours - dailyHourPrices.Count()));
        }
        return operatingHours;  
    }

    public async Task<decimal> GetDailyAverageHourPrice(DateTime date)
    {
        return (await GetDailyHourPricesByDateAsync(date)).Average(dhp => dhp.Price);
    }

    private List<IDailyHourPrice> GetPeriodsAboveAveragePrice(List<IDailyHourPrice> dailyHourPrices)
    {
        decimal averagePrice = dailyHourPrices.Average(dhp => dhp.Price);
        return dailyHourPrices.Where(dhp => dhp.Price >= averagePrice).ToList();
    }

    private List<IDailyHourPrice> GetPeriodsBelowAveragePrice(List<IDailyHourPrice> dailyHourPrices)
    {
        decimal averagePrice = dailyHourPrices.Average(dhp => dhp.Price);
        return dailyHourPrices.Where(dhp => dhp.Price < averagePrice).ToList();
    }

    private List<IDailyHourPrice> GetPeakHours(List<IDailyHourPrice> dailyHourPrices)
    {
        var peakHours = new List<IDailyHourPrice>();
        for (var i = 0; i < 24; i++)
        {
            switch (i)
            {
                case 0:
                    if (dailyHourPrices.First(dhp => dhp.Hour == 0).Price > dailyHourPrices[1].Price)
                        peakHours.Add(dailyHourPrices[0]);
                    break;
                case 23:
                    if (dailyHourPrices.First(dhp => dhp.Hour == 23).Price >
                        dailyHourPrices.First(dhp => dhp.Hour == 22).Price)
                        peakHours.Add(dailyHourPrices.First(dhp => dhp.Hour == 23));
                    break;
                case < 23 and > 0:
                    if (dailyHourPrices.First(dhp => dhp.Hour == i).Price >
                        dailyHourPrices.First(dhp => dhp.Hour == i - 1).Price &&
                        dailyHourPrices.First(dhp => dhp.Hour == i).Price >
                        dailyHourPrices.First(dhp => dhp.Hour == i + 1).Price)
                        peakHours.Add(dailyHourPrices.First(dhp => dhp.Hour == i));
                    break;
            }
        }

        return peakHours;
    }

    private List<IDailyHourPrice> GetBottomHours(List<IDailyHourPrice> dailyHourPrices)
    {
        var bottomHours = new List<IDailyHourPrice>();
        int counter = dailyHourPrices.Count();
        for (var i = 0; i < counter; i++)
        {
            if (i == 0)
            {
            }
            else if (i == counter - 1)
            {
            }
            else
            {
                if (dailyHourPrices.First(pph => pph.Hour == i).Price <
                    dailyHourPrices.First(dhp => dhp.Hour == i - 1).Price
                    && dailyHourPrices.First(dhp => dhp.Hour == i).Price <
                    dailyHourPrices.First(dhp => dhp.Hour == i + 1).Price)
                    bottomHours.Add(dailyHourPrices.First(dhp => dhp.Hour == i));
            }
        }

        return bottomHours;
    }

    private async Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesFromHvaKosterStrommenByDate(DateTime date)
    {
        return await _hvaKosterStrommenHourPriceService.GetHourPricesByDate(date);
    }

    private IEnumerable<IDailyHourPrice> AdjustPricesAccordingToGovernmentSubsidies(
        IEnumerable<IDailyHourPrice> dailyHourPrices)
    {
        dailyHourPrices = dailyHourPrices.ToList();
        foreach (var dailyHourPrice in dailyHourPrices)
        {
            dailyHourPrice.Price = GetAdjustedPrice(dailyHourPrice.Price, dailyHourPrice.Date);
        }

        return dailyHourPrices;
    }

    private decimal GetAdjustedPrice(decimal hourPrice, DateTime date)
    {
        if (date >= new DateTime(2024, 1, 1))
        {
            if (hourPrice > 0.73m)
                return ((hourPrice - 0.73m) * 0.1m) + 0.73m;

            return hourPrice;
        }
        else if (date >= new DateTime(2023, 9, 1))
        {
            if (hourPrice > 0.70m)
                return ((hourPrice - 0.70m) * 0.1m) + 0.70m;

            return hourPrice;
        }
        else if (date >= new DateTime(2023, 4, 1))
        {
            //80% av månedlig gjennomsnitt
            return hourPrice;
        }
        else if (date >= new DateTime(2022, 9, 1))
        {
            //90% av måndelig gjennomsnitt
            return hourPrice;
        }

        return hourPrice;
    }
}