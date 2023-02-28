using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.PostgreSql.DTOs;

public class DailyConsumption : IDailyConsumption
{
    public DateTime CalculationDate { get; set; }
    public double Consumption { get; set; }
    public double Cost { get; set; }
}