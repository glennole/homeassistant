using Dapper;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HomeAssistant.PostgreSql.Repositories;

public class HeavyDutySwitchRepository : IHeavyDutySwitchRepository
{
    private string ConnectionString { get; }

    public HeavyDutySwitchRepository(IConfiguration configuration)
    {
        ConnectionString = configuration["Postgresql:ConnectionString"];    
    }
    
    public Task<IEnumerable<IHeavyDutySwitch>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IDailyConsumption> GetDailyConsumptionByDate(DateTime date)
    {
        string yesterday = $"{date.Year}-{date.Month}-{date.Day}";
        string sql = "SELECT * FROM getDailyConsumption(@Date)";
        await using var con = new NpgsqlConnection(ConnectionString);
        DailyConsumption? result = await con.QueryFirstAsync<DailyConsumption>(sql, new { Date = date});
        if(result != null)
        {
            return result;
        }
        throw new ArgumentException("There are no readings from this date", "date");
    }
    
    public async Task<IHeavyDutySwitch> GetByIdAsync(int id)
    {
        string sql = "SELECT * FROM heavy_duty_switch WHERE id = @Id";
        await using var con = new NpgsqlConnection(ConnectionString);
        return await con.QueryFirstAsync<HeavyDutySwitch>(sql, new { Id = id}).ConfigureAwait(false);
    }

    public async Task<IHeavyDutySwitch> AddAsync(IHeavyDutySwitch item)
    {
        string sql = "INSERT INTO heavy_duty_switch(heavy_duty_switch_id, reading_at, accumulated_kwh, accumulated_kwh_last_changed_at, state, state_last_changed_at) VALUES(@HeavyDutySwitchId, @ReadingAt, @AccumulatedKwh, @AccumulatedKwhLastChangedAt, @State, @StateLastChangedAt) RETURNING id";
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