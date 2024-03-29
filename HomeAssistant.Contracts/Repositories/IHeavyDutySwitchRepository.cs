using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Contracts.Repositories;

public interface IHeavyDutySwitchRepository : IBaseRepository<IHeavyDutySwitch>
{
    Task<IDailyConsumption> GetDailyConsumptionByDateAsync(DateTime date);
    Task<IEnumerable<IHeavyDutySwitch>> GetReadingsPerHourByDateAsync(DateTime date);
}