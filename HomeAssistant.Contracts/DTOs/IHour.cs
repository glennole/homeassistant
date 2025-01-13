namespace HomeAssistant.Contracts.DTOs;

public interface IHour
{
    int Id { get; set; }
    DateTime Date { get; set; }
    DateTime StartAt { get; set; }
    DateTime EndAt { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime CreatedAt { get; set; }
}