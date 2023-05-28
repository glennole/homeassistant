using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;

namespace HomeAssistant.Service.Services;

public interface IWaterHeaterService
{
    Task<State> GetStateById(int id);
    Task<IHeavyDutySwitch> GetById(int id);
}

public class WaterHeaterService : IWaterHeaterService
{
    private readonly IHeavyDutySwitchRepository _heavyDutySwitchRepository;
    
    public WaterHeaterService(IHeavyDutySwitchRepository heavyDutySwitchRepository)
    {
        _heavyDutySwitchRepository = heavyDutySwitchRepository;
    }

    public async Task<IHeavyDutySwitch> GetById(int id)
    {
        return await _heavyDutySwitchRepository.GetByIdAsync(id);
    }
    public async Task<State> GetStateById(int id)
    {
        IHeavyDutySwitch heavyDutySwitch = await _heavyDutySwitchRepository.GetByIdAsync(id);
        State state = State.Unknown;
        if (heavyDutySwitch != null)
            Enum.TryParse<State>(heavyDutySwitch.State, out state);
        return state;
    }
}