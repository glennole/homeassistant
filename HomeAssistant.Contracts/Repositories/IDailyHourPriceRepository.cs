using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Contracts.Repositories;

public interface IDailyHourPriceRepository : IBaseRepository<IDailyHourPrice>
{
    Task<bool> HasPricesForGivenDate(DateTime date);
    Task<DateTime> GetLastDailyHourDate();
    Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesByDate(DateTime date);
}