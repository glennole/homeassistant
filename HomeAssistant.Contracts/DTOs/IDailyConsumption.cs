namespace HomeAssistant.Contracts.DTOs;

public interface IDailyConsumption
{
    DateTime CalculationDate { get; set; }
    double Consumption { get; set; }
    double Cost { get; set; }
}