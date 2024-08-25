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

    public HeavyDutySwitchRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    public Task<IEnumerable<IHeavyDutySwitch>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IDailyConsumption> GetDailyConsumptionByDateAsync(DateTime date)
    {
        string sql = "SELECT * FROM getDailyConsumption(@Date)";
        await using var con = new NpgsqlConnection(ConnectionString);
        DailyConsumption? result = await con.QueryFirstOrDefaultAsync<DailyConsumption>(sql, new { Date = date});
        if(result != null)
        {
            return result;
        }
        throw new ArgumentException("There are no readings from this date", "date");
    }

    public async Task<IEnumerable<IHeavyDutySwitch>> GetReadingsByDateAsync(DateTime date)
    {
        
        DateTime fromDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Local);
        DateTime toDate = new DateTime(date.Year, date.Month, date.Day, 1, 0, 0, DateTimeKind.Local).AddDays(1);
        
        string sql = "SELECT * FROM heavy_duty_switch WHERE reading_at between @from and @to ORDER BY reading_at ASC";
        await using var con = new NpgsqlConnection(ConnectionString);
        return await con.QueryAsync<HeavyDutySwitch>(sql, new { From = fromDate, To = toDate });
    }

    public async Task<IHeavyDutySwitch> GetPreviousReadingAsync(DateTime date)
    {
        string sql = "SELECT * FROM heavy_duty_switch WHERE reading_at < @date ORDER BY reading_at DESC LIMIT 1";
        await using var con = new NpgsqlConnection(ConnectionString);
        List<HeavyDutySwitch> readings =  (await con.QueryAsync<HeavyDutySwitch>(sql, new { Date = date})).ToList();
        return readings.Any() ? readings.First() : null;
    }

    public async Task<IHeavyDutySwitch> GetNextReadingAsync(DateTime date)
    {
        string sql = "SELECT * FROM heavy_duty_switch WHERE reading_at > @date ORDER BY reading_at ASC LIMIT 1";
        await using var con = new NpgsqlConnection(ConnectionString);
        List<HeavyDutySwitch> readings =  (await con.QueryAsync<HeavyDutySwitch>(sql, new { Date = date})).ToList();
        return readings.Any() ? readings.First() : null;
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