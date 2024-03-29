using Dapper;
using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.PostgreSql.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace HomeAssistant.PostgreSql.Repositories;

public class DailyHourPriceRepository : IDailyHourPriceRepository
{
    private string ConnectionString { get; }
    
    public DailyHourPriceRepository(IConfiguration configuration)
    {
        ConnectionString = configuration["Postgresql:ConnectionString"];
    }
    public Task<IEnumerable<IDailyHourPrice>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IDailyHourPrice> GetByIdAsync(int id)
    {
        string sql = "SELECT * FROM daily_hour_price WHERE id = @Id";
        await using var con = new NpgsqlConnection(ConnectionString);
        return await con.QueryFirstAsync<DailyHourPrice>(sql, new { Id = id});

    }

    public async Task<IDailyHourPrice> AddAsync(IDailyHourPrice item)
    {
        string sql = "INSERT INTO daily_hour_price(date, hour, price, description) VALUES(@Date, @Hour, @Price, @Description) RETURNING id";
        await using var con = new NpgsqlConnection(ConnectionString);
        var dailyHourPriceId = await con.QueryFirstAsync<int>(sql, item);
        return await GetByIdAsync(dailyHourPriceId);
    }

    public Task<IDailyHourPrice> UpdateAsync(IDailyHourPrice item)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(IDailyHourPrice item)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> HasPricesForGivenDate(DateTime date)
    {
        var dailyHourPrices = await FetchDailyHourPricesByDate(date);
        return dailyHourPrices.Any();
    }

    public async Task<DateTime> GetLastDailyHourDate()
    {
        string sql = "SELECT MAX(date) FROM daily_hour_price";

        using var con = new NpgsqlConnection(ConnectionString);
        return await con.QueryFirstAsync<DateTime>(sql);
        
    }

    public async Task<IEnumerable<IDailyHourPrice>> GetDailyHourPricesByDate(DateTime date)
    {
        return await FetchDailyHourPricesByDate(date);
    }

    private async Task<IEnumerable<IDailyHourPrice>> FetchDailyHourPricesByDate(DateTime date)
    {
        string sql = "SELECT * FROM daily_hour_price WHERE date = @Date::date";
        using var con = new NpgsqlConnection(ConnectionString);
        return  await con.QueryAsync<DailyHourPrice>(sql, new { Date = date});
    }
}