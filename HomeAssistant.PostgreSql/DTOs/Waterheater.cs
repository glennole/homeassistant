using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.PostgreSql.DTOs;

public class Waterheater : IWaterheater
{
    public int Id { get; set; }
    public string HeavyDutySwitchId { get; set; }
    public string Name { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}