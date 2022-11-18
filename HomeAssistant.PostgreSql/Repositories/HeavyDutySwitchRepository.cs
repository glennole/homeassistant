using Dapper;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HomeAssistant.PostgreSql.Repositories;

public class HeavyDutySwitchRepository : IHeavyDutySwitchRepository
{
    private string ConnectionString { get; }

    public HeavyDutySwitchRepository(IConfiguration configuration)
    {
        ConnectionString = configuration["PostgresqlOptions:ConnectionString"];    
    }
    
    public Task<IEnumerable<IHeavyDutySwitch>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IHeavyDutySwitch> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IHeavyDutySwitch> AddAsync(IHeavyDutySwitch item)
    {
        string sql = "INSERT INTO heavy_duty_switch(heavy_duty_switch_id, reading_at, accumulated_kwh, accumulated_kwh_last_changed_at, state, state_last_changed_at) VALUES(@HeavyDutySwitchId, @ReadingAt, @AccumulatedKwh, @AccumulatedKwhLastChangedAt, @State, @StateLastChangedAt)";
        await using var con = new NpgsqlConnection(ConnectionString);
        var heavySwitchId = await con.QueryFirstAsync<int>(sql, item);
        return await GetByIdAsync(heavySwitchId);
    }

    public Task<IHeavyDutySwitch> UpdateAsync(IHeavyDutySwitch item)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(IHeavyDutySwitch item)
    {
        throw new NotImplementedException();
    }
}