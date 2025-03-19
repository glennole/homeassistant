using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;

namespace HomeAssistant.Tests;

public class HourRepositoryMocked : IHourRepository
{
    public Task<IHour> AddHourAsync(IHour hour)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IHour>> GetHoursByDateAsync(DateTime date)
    {
        throw new NotImplementedException();
    }
}