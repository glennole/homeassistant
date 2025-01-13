using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Contracts.Repositories;

public interface IHourRepository
{
    Task<IHour> AddHourAsync(IHour hour);
    Task<IEnumerable<IHour>> GetHoursByDateAsync(DateTime date);
}