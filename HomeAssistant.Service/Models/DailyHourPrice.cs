using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Service.Models;

public class DailyHourPrice : IDailyHourPrice
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Hour { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}