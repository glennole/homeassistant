using System.Data;
using System.Runtime.Serialization.Formatters;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Service.Configuration;
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
    Task<List<int>> GetOperatingHoursByDate(DateTime date);
}

public class WaterHeaterService : IWaterHeaterService
{
    private readonly IHeavyDutySwitchRepository _heavyDutySwitchRepository;
    private readonly IDailyHourPriceService _dailyHourPriceService;

    public WaterHeaterService(IHeavyDutySwitchRepository heavyDutySwitchRepository,
        IDailyHourPriceService dailyHourPriceService)
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

        DateTime date = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Local);
        decimal sumSaved = 0.0m;

        while (date.Month == month)
        {
            sumSaved += await GetSavedByDateAsync(date);
            date = date.AddDays(1);
        }

        return sumSaved;
    }

    public async Task<List<int>> GetOperatingHoursByDate(DateTime date)
    {
        IEnumerable<IDailyHourPrice> result = await _dailyHourPriceService.GetDailyHourPricesByDateAsync(date);

        return result.OrderBy(t => t.Price).Take(5).Select(t => t.Hour).ToList();
    }


    private async Task<IEnumerable<HourlyConsumptionWithPriceAndCost>> GetHourlyConsumptionAndPriceByDate(DateTime date)
    {
        var waterReadingTask = _heavyDutySwitchRepository.GetReadingsByDateAsync(date);
        var hourPricesTask = _dailyHourPriceService.GetDailyHourPricesByDateAsync(date);

        await Task.WhenAll(waterReadingTask, hourPricesTask);

        List<IHeavyDutySwitch> waterHeaterReadings = waterReadingTask.Result.ToList();
        List<IDailyHourPrice> hourPrices = hourPricesTask.Result.OrderBy(hp => hp.Hour).ToList();


        var previousReadingTask = _heavyDutySwitchRepository.GetPreviousReadingAsync(waterHeaterReadings.Any()
            ? waterHeaterReadings.OrderBy(r => r.ReadingAt).First().ReadingAt
            : date);
        var nextReadingTask = _heavyDutySwitchRepository.GetNextReadingAsync(waterHeaterReadings.Any()
            ? waterHeaterReadings.OrderByDescending(r => r.ReadingAt).First().ReadingAt
            : date);

        await Task.WhenAll(previousReadingTask, nextReadingTask);

        IHeavyDutySwitch previousReading = previousReadingTask.Result;
        IHeavyDutySwitch nextReading = nextReadingTask.Result;

        if (hourPrices.Count() < 24)
        {
            //Log.Information();
            throw new ArgumentException("Unable to retrieve readings for given date");
        }

        List<HourlyConsumptionWithPriceAndCost> hourlyConsumptions = new List<HourlyConsumptionWithPriceAndCost>();

        bool setAllToZero = !waterHeaterReadings.Any();

        for (int i = 0; i < 24; i++)
        {
            if (setAllToZero)
            {
                hourlyConsumptions.Add(
                    new HourlyConsumptionWithPriceAndCost(i, 0, hourPrices.First(h => h.Hour == i).Price));
            }
            else
            {
                if (!waterHeaterReadings.Any(r => r.ReadingAt >= date && r.ReadingAt < date.AddHours(1))
                    ||
                    (!waterHeaterReadings.Any(r => r.ReadingAt >= date.AddHours(1)) && nextReading == null))
                {
                    hourlyConsumptions.Add(
                        new HourlyConsumptionWithPriceAndCost(i, 0, hourPrices.First(h => h.Hour == i).Price));
                }
                else if (waterHeaterReadings.Any(r => r.ReadingAt >= date.AddHours(1)))
                {
                    decimal consumption = waterHeaterReadings.Where(r => r.ReadingAt >= date.AddHours(1))
                                              .OrderBy(r => r.ReadingAt).First().AccumulatedKwh -
                                          waterHeaterReadings
                                              .Where(r => r.ReadingAt >= date && r.ReadingAt < date.AddHours(1))
                                              .OrderBy(r => r.ReadingAt)
                                              .First().AccumulatedKwh;
                    hourlyConsumptions.Add(
                        new HourlyConsumptionWithPriceAndCost(i, consumption, hourPrices.First(h => h.Hour == i).Price));
                }
                else
                {
                    decimal consumption = nextReading.AccumulatedKwh -
                                          waterHeaterReadings
                                              .Where(r => r.ReadingAt >= date && r.ReadingAt < date.AddHours(1))
                                              .OrderBy(r => r.ReadingAt)
                                              .First().AccumulatedKwh;
                    hourlyConsumptions.Add(
                        new HourlyConsumptionWithPriceAndCost(i, consumption, hourPrices.First(h => h.Hour == i).Price));
                }
            }

            date = date.AddHours(1);
        }

        return hourlyConsumptions;
    }
}