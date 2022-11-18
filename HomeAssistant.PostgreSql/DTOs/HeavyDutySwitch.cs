using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.PostgreSql.DTOs;

public class HeavyDutySwitch : IHeavyDutySwitch
{
    public int Id { get; set; }
    public string HeavyDutySwitchId { get; set; }
    public DateTime ReadingAt { get; set; }
    public string State { get; set; }
    public DateTime StateLastChangedAt { get; set; }
    public decimal AccumulatedKwh { get; set; }
    public DateTime AccumulatedKwhLastChangedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}