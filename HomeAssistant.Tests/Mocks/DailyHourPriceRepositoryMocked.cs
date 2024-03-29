using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;

namespace HomeAssistant.Tests;

public class DailyHourPriceRepositoryMocked : IDailyHourPriceRepository
{

    private readonly List<IDailyHourPrice> _dailyHourPrices = new List<IDailyHourPrice>();
    
    public async Task<IEnumerable<IDailyHourPrice>> GetAsync()
    {
        return await Task.FromResult<IEnumerable<IDailyHourPrice>>(_dailyHourPrices);
    }

    public async Task<IDailyHourPrice> GetByIdAsync(int id)
    {
        return await Task.FromResult(_dailyHourPrices.First(dhp => dhp.Id == id));
    }

    public async Task<IDailyHourPrice> AddAsync(IDailyHourPrice item)
    {
        item.Id = _dailyHourPrices.Count() + 1;
        item.CreatedAt = DateTime.Now;
        _dailyHourPrices.Add(item);
        return await GetByIdAsync(item.Id);
    }

    public Task<IDailyHourPrice> UpdateAsync(IDailyHourPrice item)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(IDailyHourPrice item)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> HasPricesForGivenDate(DateTime date)
    {
        return await Task.FromResult(_dailyHourPrices.Any(dhp => dhp.Date.Date == date.Date));
    }

    public async Task<DateTime> GetLastDailyHourDate()
    {
        return await Task.FromResult(_dailyHourPrices.OrderByDescending(dhp => dhp.Date).First().Date);
    }

    public Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesByDate(DateTime date)
    {
        throw new NotImplementedException();
    }
}