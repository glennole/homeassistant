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
}

public class DailyHourPriceService : IDailyHourPriceService
{
    private readonly IDailyHourPriceRepository _dailyHourPriceRepository;
    private readonly IHvaKosterStrommenHourPriceService _hvaKosterStrommenHourPriceService;
    

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
            IEnumerable<IDailyHourPrice> dailyHourPrices = await GetDailyHourPricesFromHvaKosterStrommenByDate(lastDailyHourDate.AddDays(1));
            foreach (IDailyHourPrice dailyHourPrice in dailyHourPrices)
            {
                await _dailyHourPriceRepository.AddAsync(dailyHourPrice);
            }
            lastDailyHourDate = lastDailyHourDate.AddDays(1);
            counter++;
        }

        if (lastDailyHourDate.Date == DateTime.Now.Date && DateTime.Now.Hour > 13)
        {
            IEnumerable<IDailyHourPrice> dailyHourPrices = await GetDailyHourPricesFromHvaKosterStrommenByDate(lastDailyHourDate.AddDays(1));
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
        return AdjustPricesAccordingToGovernmentSubsidies(await _dailyHourPriceRepository.GetDailyHourPricesByDate(date));
    }

    public async Task<int> FetchAndStoreDailyHourPricesByDateAsync(DateTime date)
    {
        IEnumerable<IDailyHourPrice> existingDailyHourPrices = await _dailyHourPriceRepository.GetDailyHourPricesByDate(date);
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

    private async Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesFromHvaKosterStrommenByDate(DateTime date)
    {
        return await _hvaKosterStrommenHourPriceService.GetHourPricesByDate(date);
    }
    
    private IEnumerable<IDailyHourPrice> AdjustPricesAccordingToGovernmentSubsidies(IEnumerable<IDailyHourPrice> dailyHourPrices)
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