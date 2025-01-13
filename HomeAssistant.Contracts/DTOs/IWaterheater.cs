namespace HomeAssistant.Contracts.DTOs;

public interface IWaterheater
{
    int Id { get; set; }
    string HeavyDutySwitchId { get; set; }
    string Name { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime CreatedAt { get; set; }
}