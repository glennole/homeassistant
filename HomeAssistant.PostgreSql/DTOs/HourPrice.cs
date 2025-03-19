using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.PostgreSql.DTOs;

public class HourPrice : IHourPrice
{
    public int Id { get; set; }
    public int HourId { get; set; }
    public decimal Price { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}