using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Service.DTOs;

namespace HomeAssistant.Service.Models;

public class HeavyDutySwitch
{
    public int Id { get; set; }
    public string HeavyDutySwitchId { get; set; }
    public DateTime ReadingAt { get; set; }
    public State State { get; set; }
    public DateTime StateLastChangedAt { get; set; }
    public decimal AccumulatedKwh { get; set; }
    public DateTime AccumulatedKwhLastChangedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public IHeavyDutySwitch MapToDto()
    {
        return new HeavyDutySwitchDto()
        {
            AccumulatedKwh = AccumulatedKwh,
            Id = Id,
            State = State == State.On ? "On" : "Off",
            CreatedAt = CreatedAt,
            ReadingAt = ReadingAt,
            HeavyDutySwitchId = HeavyDutySwitchId,
            StateLastChangedAt = StateLastChangedAt,
            AccumulatedKwhLastChangedAt = AccumulatedKwhLastChangedAt
        };
    }
}