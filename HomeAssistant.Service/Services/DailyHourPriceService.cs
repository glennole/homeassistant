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
        return await _dailyHourPriceRepository.GetDailyHourPricesByDate(date);
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

}