using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;

namespace HomeAssistant.Tests;

public class DailyHourPriceRepositoryMocked : IDailyHourPriceRepository
{
    private readonly List<IDailyHourPrice> _dailyHourPrices = new List<IDailyHourPrice>();

    public DailyHourPriceRepositoryMocked()
    {
        List<decimal> october10th = new List<decimal>()
        {
            0.643m, 0.636m, 0.633m, 0.633m, 0.634m, 0.643m,
            0.714m, 1.904m, 2.438m, 1.566m, 0.876m, 0.712m,
            0.701m, 0.691m, 0.666m, 0.665m, 0.670m, 0.713m,
            0.892m, 0.906m, 0.714m, 0.714m, 0.666m, 0.6m
        };
        for (int i = 0; i < october10th.Count; i++)
            _dailyHourPrices.Add(CreateDailyHourPrice(new DateTime(2024,10,18), i, october10th[i]));
    }
    
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

    public async Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesByDate(DateTime date)
    {
        return _dailyHourPrices.Where(dhp => dhp.Date.Date == date.Date);
        List<IDailyHourPrice> dailyHourPrices = new List<IDailyHourPrice>();
        
        List<decimal> pricesOnePeak = new List<decimal>()
        {
            0.256m, 0.258m, 0.265m, 0.400m, 0.450m, 0.384m,
            0.376m, 0.360m, 0.350m, 0.340m, 0.335m, 0.334m,
            0.333m, 0.300m, 0.289m, 0.280m, 0.274m, 0.270m,
            0.268m, 0.267m, 0.267m, 0.264m, 0.260m, 0.258m
        }; 
        
       for (int i = 0; i < pricesOnePeak.Count; i++)
           dailyHourPrices.Add(CreateDailyHourPrice(date, i, pricesOnePeak[i]));
       
       return await Task.FromResult(dailyHourPrices);
    }

    private IDailyHourPrice CreateDailyHourPrice(DateTime date, int hour, decimal price)
    {
        return new DailyHourPrice()
        {
            Id = date.Day * 100 + hour,
            CreatedAt = DateTime.Now,
            Date = date,
            Description = $"[{hour}, {hour + 1}>",
            Hour = hour,
            Price = price
        };
    }
}