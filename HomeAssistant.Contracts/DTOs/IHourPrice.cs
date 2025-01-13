namespace HomeAssistant.Contracts.DTOs;

public interface IHourPrice
{
    int Id { get; set; }
    int HourId { get; set; }
    decimal Price { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime CreatedAt { get; set; }
}