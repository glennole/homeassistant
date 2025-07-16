using System.Text.Json;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Service.Models;
using HomeAssistant.Service.Services;
using Serilog;

namespace HomeAssistant.Service.HvaKosterStrommen;

public interface IHvaKosterStrommenHourPriceService
{
    Task<IEnumerable<IDailyHourPrice>> GetHourPricesByDate(DateTime date);
}

public class HvaKosterStrommenHourPriceService : IHvaKosterStrommenHourPriceService
{
    private readonly HttpClient _httpClient;
    private readonly IEmailService _emailService;
    public HvaKosterStrommenHourPriceService(IEmailService emailService)
    {
        _emailService = emailService;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://www.hvakosterstrommen.no/api/v1/prices/");
    }

    public async Task<IEnumerable<IDailyHourPrice>> GetHourPricesByDate(DateTime date)
    {
        List<DailyHourPrice> dailyHourPrices = new List<DailyHourPrice>();
        try
        {
            string dateFormatted = date.ToString("yyyy/MM-dd");
            Log.Information("Fetching hour prices from HvaKosterStrommen for {@date}.", date);
            HttpResponseMessage response = await _httpClient.GetAsync($"{dateFormatted}_NO2.json");
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            List<HvaKosterStrommenHourPrice> hourPrices =
                JsonSerializer.Deserialize<List<HvaKosterStrommenHourPrice>>(responseJson);

            if (hourPrices.Count() == 23)
                hourPrices.Insert(2, hourPrices[1]);

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
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Failed to fetch hour prices from {@date}.", date);
            if(_emailService != null)
                await _emailService.SendEmail($"HomeAssistant - Hva koster strømmen feilet ved henting av priser for {date}.", ex.ToString(), null);
        }

        return dailyHourPrices;
    }
}