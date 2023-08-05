using System.Text.Json;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Service.Models;
using Serilog;

namespace HomeAssistant.Service.HvaKosterStrommen;

public interface IHvaKosterStrommenHourPriceService
{
    Task<IEnumerable<IDailyHourPrice>> GetHourPricesByDate(DateTime date);
}

public class HvaKosterStrommenHourPriceService : IHvaKosterStrommenHourPriceService
{
    private readonly HttpClient _httpClient;

    public HvaKosterStrommenHourPriceService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://www.hvakosterstrommen.no/api/v1/prices/");
    }

    public async Task<IEnumerable<IDailyHourPrice>> GetHourPricesByDate(DateTime date)
    {
        string dateFormatted = date.ToString("yyyy/MM-dd");
        Log.Information("Fetching hour prices from HvaKosterStrommen for {@date}.", date);
        HttpResponseMessage response = await _httpClient.GetAsync($"{dateFormatted}_NO2.json");
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();

        List<HvaKosterStrommenHourPrice> hourPrices = JsonSerializer.Deserialize<List<HvaKosterStrommenHourPrice>>(responseJson);

        List<DailyHourPrice> dailyHourPrices = new List<DailyHourPrice>();

        for (int i = 0; i < 24; i++)
        {
            dailyHourPrices.Add(new DailyHourPrice()
            {
                Date = hourPrices[i].time_start,
                Description = $"[{i}, {i + 1}>",
                Hour = i,
                Price = hourPrices[i].NOK_per_kWh
            });
        }

        return dailyHourPrices;
    }
}