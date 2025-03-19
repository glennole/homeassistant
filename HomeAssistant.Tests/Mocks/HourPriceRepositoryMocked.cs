using System.Threading.Tasks;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;

namespace HomeAssistant.Tests;

public class HourPriceRepositoryMocked : IHourPriceRepository
{
    public Task<IHourPrice> AddHourPriceAsync(IHourPrice hourPrice)
    {
        throw new System.NotImplementedException();
    }
}