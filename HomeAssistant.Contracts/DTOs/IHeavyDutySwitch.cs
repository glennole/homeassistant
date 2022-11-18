namespace HomeAssistant.Contracts.DTOs;

public interface IHeavyDutySwitch
{
    int Id { get; set; }
    string HeavyDutySwitchId { get; set; }
    DateTime ReadingAt { get; set; }
    string State { get; set; }
    DateTime StateLastChangedAt { get; set; }
    decimal AccumulatedKwh { get; set; }
    DateTime AccumulatedKwhLastChangedAt { get; set; }
    DateTime CreatedAt { get; set; }
}