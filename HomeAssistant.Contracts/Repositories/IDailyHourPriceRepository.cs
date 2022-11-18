using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Contracts.Repositories;

public interface IDailyHourPriceRepository : IBaseRepository<IDailyHourPrice>
{
    Task<bool> HasPricesForGivenDate(DateTime date);
}