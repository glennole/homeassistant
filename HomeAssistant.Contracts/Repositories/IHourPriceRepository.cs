using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Contracts.Repositories;

public interface IHourPriceRepository
{
    Task<IHourPrice> AddHourPriceAsync(IHourPrice hourPrice);
}