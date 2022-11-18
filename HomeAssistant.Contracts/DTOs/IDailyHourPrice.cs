namespace HomeAssistant.Contracts.DTOs;

public interface IDailyHourPrice
{
    int Id { get; set; }
    DateTime Date { get; set; }
    int Hour { get; set; }
    decimal Price { get; set; }
    string Description { get; set; }
    DateTime CreatedAt { get; set; }
}