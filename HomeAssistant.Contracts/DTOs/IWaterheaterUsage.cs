namespace HomeAssistant.Contracts.DTOs;

public interface IWaterheaterUsage
{
    int Id { get; set; }
    int WaterheaterId { get; set; }
    int HourId { get; set; }
    bool CalculatedState { get; set; }
    bool? OverriddenState { get; set; }
    decimal? Consumption { get; set; }
    decimal? Cost { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime CreatedAt { get; set; }
}