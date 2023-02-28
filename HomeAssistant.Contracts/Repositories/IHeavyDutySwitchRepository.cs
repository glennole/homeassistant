using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Contracts.Repositories;

public interface IHeavyDutySwitchRepository : IBaseRepository<IHeavyDutySwitch>
{
    Task<IDailyConsumption> GetDailyConsumptionByDate(DateTime date);
}