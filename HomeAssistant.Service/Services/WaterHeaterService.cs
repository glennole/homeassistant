using System.Runtime.Serialization.Formatters;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Service.Models;

namespace HomeAssistant.Service.Services;

public interface IWaterHeaterService
{
    Task<State> GetStateByIdAsync(int id);
    Task<IHeavyDutySwitch> GetByIdAsync(int id);
    Task<decimal> GetWaterHeaterCostByDateAsync(DateTime date);
    Task<decimal> GetWaterHeaterConsumptionByDateAsync(DateTime date);
    Task<decimal> GetSavedByDateAsync(DateTime date);
    Task<decimal> GetSavedByMonthAsync(int year, int month);
}

public class WaterHeaterService : IWaterHeaterService
{
    private readonly IHeavyDutySwitchRepository _heavyDutySwitchRepository;
    private readonly IDailyHourPriceService _dailyHourPriceService;
    
    public WaterHeaterService(IHeavyDutySwitchRepository heavyDutySwitchRepository, IDailyHourPriceService dailyHourPriceService)
    {
        _heavyDutySwitchRepository = heavyDutySwitchRepository;
        _dailyHourPriceService = dailyHourPriceService;
    }

    public async Task<IHeavyDutySwitch> GetByIdAsync(int id)
    {
        return await _heavyDutySwitchRepository.GetByIdAsync(id);
    }
    public async Task<State> GetStateByIdAsync(int id)
    {
        IHeavyDutySwitch heavyDutySwitch = await _heavyDutySwitchRepository.GetByIdAsync(id);
        State state = State.Unknown;
        if (heavyDutySwitch != null)
            Enum.TryParse<State>(heavyDutySwitch.State, out state);
        return state;
    }
    
    public async Task<decimal> GetWaterHeaterCostByDateAsync(DateTime date)
    {
        return (await GetHourlyConsumptionAndPriceByDate(date)).Sum(hc => hc.Cost);
    }

    public async Task<decimal> GetWaterHeaterConsumptionByDateAsync(DateTime date)
    {
        return (await GetHourlyConsumptionAndPriceByDate(date)).Sum(hc => hc.Consumption);
    }

    public async Task<decimal> GetSavedByDateAsync(DateTime date)
    {
        var todaysReadingsTask = GetHourlyConsumptionAndPriceByDate(date);
        var tomorrowsReadingsTask = GetHourlyConsumptionAndPriceByDate(date.AddDays(1));

        await Task.WhenAll(todaysReadingsTask, tomorrowsReadingsTask);

        var todaysReadings = todaysReadingsTask.Result.ToList();
        var tomorrowsReadings = tomorrowsReadingsTask.Result.ToList();

        var temp = todaysReadings.Where(tr => tr.Hour > 5).ToList();
        temp.AddRange(tomorrowsReadings.Where(tr => tr.Hour < 6));
        
        var calculationReadings = new List<HourlyConsumptionWithPriceAndCost>();
        int counter = 6;
        foreach (var hourlyConsumptionWithPriceAndCost in temp)
        {
            if (hourlyConsumptionWithPriceAndCost.Consumption > 0)
            {
                var reading = new HourlyConsumptionWithPriceAndCost(counter,
                    hourlyConsumptionWithPriceAndCost.Consumption, temp.First(r => r.Hour == counter).Price);
                calculationReadings.Add(reading);
                counter++;
            }
        }

        return calculationReadings.Sum(r => r.Cost) - todaysReadings.Sum(r => r.Cost);
    }

    public async Task<decimal> GetSavedByMonthAsync(int year, int month)
    {
        if (year > DateTime.Now.Year || (DateTime.Now.Year == year && month >= DateTime.Now.Month))
            throw new ArgumentException("Only months in the past is valid");
        
        DateTime date = new DateTime(year, month, 1, 0,0,0, DateTimeKind.Local);
        decimal sumSaved = 0.0m;
        
        while(date.Month == month)
        {
            sumSaved += await GetSavedByDateAsync(date);
            date = date.AddDays(1);
        }
        
        return sumSaved;
    }


    private async Task<IEnumerable<HourlyConsumptionWithPriceAndCost>> GetHourlyConsumptionAndPriceByDate(DateTime date)
    {
        var waterReadingTask = _heavyDutySwitchRepository.GetReadingsPerHourByDateAsync(date);
        var hourPricesTask = _dailyHourPriceService.GetDailyHourPricesByDateAsync(date);
        await Task.WhenAll(waterReadingTask, hourPricesTask);
        
        List<IHeavyDutySwitch> waterHeaterReadings = waterReadingTask.Result.ToList();
        List<IDailyHourPrice> hourPrices = hourPricesTask.Result.OrderBy(hp => hp.Hour).ToList();

        if (waterHeaterReadings.Count() < 24 || hourPrices.Count() < 24)
        {
            //Log.Information();
            throw new ArgumentException("Unable to retrieve readings for given date");
        }
        
        List<HourlyConsumptionWithPriceAndCost> hourlyConsumptions = new List<HourlyConsumptionWithPriceAndCost>();
        for (int i = 0; i < waterHeaterReadings.Count() - 1; i++)
        {
            decimal consumption = waterHeaterReadings[i + 1].AccumulatedKwh - waterHeaterReadings[i].AccumulatedKwh;
            HourlyConsumptionWithPriceAndCost hourlyConsumption =
                new HourlyConsumptionWithPriceAndCost(i, consumption, hourPrices[i].Price);
            
            hourlyConsumptions.Add(hourlyConsumption);
        }

        return hourlyConsumptions;
    }
}