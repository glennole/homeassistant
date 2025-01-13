using Dapper;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HomeAssistant.PostgreSql.Repositories;

public class WaterHeaterRepository : IWaterheaterRepository
{
    private string ConnectionString { get; }
    
    public WaterHeaterRepository(IConfiguration configuration)
    {
        ConnectionString = configuration["Postgresql:ConnectionString"];
    }
    
    public async Task<IWaterheater> AddWaterheaterAsync(IWaterheater waterheater)
    {
        string sql = "INSERT INTO waterheater(heavy_duty_switch_id, name) VALUES(@HeavyDutySwitch, @Name) RETURNING id";
        await using var con = new NpgsqlConnection(ConnectionString);
        var waterheaterId = await con.QueryFirstAsync<int>(sql, waterheater);
        return await GetWaterheaterByIdAsync(waterheaterId);
    }

    public Task<IWaterheater> UpdateWaterheaterAsync(IWaterheater waterheater)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<IWaterheater>> GetWaterheatersAsync()
    {
        string sql = "SELECT * FROM waterheater";
        await using var con = new NpgsqlConnection(ConnectionString);
        return  await con.QueryAsync<Waterheater>(sql);
    }

    public async Task<IWaterheater> GetWaterheaterByIdAsync(int waterheaterId)
    {
        string sql = "SELECT * FROM waterheater WHERE id = @Id";
        await using var con = new NpgsqlConnection(ConnectionString);
        return await con.QueryFirstAsync<Waterheater>(sql, new { Id = waterheaterId});
    }

    public Task<IWaterheaterUsage> AddWaterheaterUsage(IWaterheaterUsage waterheaterUsage)
    {
        throw new NotImplementedException();
    }

    public Task<IWaterheaterUsage> UpdateWaterheaterUsageAsync(IWaterheaterUsage waterheaterUsage)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IWaterheaterUsage>> GetWaterheaterUsagesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IWaterheaterUsage> GetWaterheaterUsageByIdAsync(int waterheaterUsageId)
    {
        throw new NotImplementedException();
    }
}